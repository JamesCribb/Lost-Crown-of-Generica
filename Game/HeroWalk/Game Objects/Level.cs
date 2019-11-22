using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;

namespace HeroWalk
{
    public class Level : IGameScreen
    {
        // Pass to ScreenManager to determine song
        private string zoneName;

        // For checking bounding boxes
        private Texture2D testing;

        // TEST: Adjust the 'lighting'
        private Color lightColor;

        // The position of the level in the zone
        public int X { get; }
        public int Y { get; }

        // Defines the size and scale of the Level
        const int numRows = 20;
        const int numCols = 20;
        const int scale = 2;
       
        public Rectangle Bounds { get; }
        public Rectangle InnerBounds { get; }

        // NB: We currently hold the texture for each tile in one array, and the location
        // to draw that texture in another array. This should probably be changed...
        private Texture2D[] tiles;
        private Rectangle[] locations;
        public List<Rectangle> TileBoundingBoxes { get; }

        public List<Prop> props;
        public List<Prop> collidableProps;
        public List<IDestructible> destructibles;
        public List<Mobile> mobiles;
        public List<WorldObject> drawables;
        public List<ZoneTrigger> zoneTriggers;
        public List<InteractibleProp> interactibles;

        public Player Hero { get; set; }

        public bool showBoundingBoxes = false;
        private KeyboardState prevKS;

        // Store these so we can rebuild the Level
        private Dictionary<string, Prop> objectDict;
        private Dictionary<string, Mobile> mobileDict;
        private Dictionary<string, InteractibleProp> interactibleDict;
        private List<string> objectMap;
        private List<string> mobileMap;
        private List<string> zoneTriggerMap;
        private List<string> interactibleMap;

        public Level(Dictionary<string, Texture2D> tileDict, List<string> tileMap, 
            Dictionary<string, List<Rectangle>> tileBBs, 
            Dictionary<string, Prop> objectDict, 
            List<string> objectMap, Dictionary<string, Mobile> mobileDict, 
            List<string> mobileMap, List<string> zoneTriggerMap,
            Dictionary<string, InteractibleProp> interactibleDict, 
            List<string> interactibleMap, Player hero, Vector2 location, Texture2D testing, 
            string zoneName, Color lightColor)
        {
            // Save these so we can rebuild the level
            this.objectDict = objectDict;
            this.mobileDict = mobileDict;
            this.interactibleDict = interactibleDict;
            this.objectMap = objectMap;
            this.mobileMap = mobileMap;
            this.interactibleMap = interactibleMap;
            this.zoneTriggerMap = zoneTriggerMap;

            this.zoneName = zoneName;
            this.lightColor = lightColor;

            this.testing = testing;
            X = (int)location.X;
            Y = (int)location.Y;
            
            Bounds = new Rectangle(0, 0, 640, 640);
            // Stops unwanted level transitions being triggered
            InnerBounds = new Rectangle(32, 32, 600, 600);

            // Initialise the tiles. No need to rebuild, since they never change
            tiles = new Texture2D[tileMap.Count];
            locations = new Rectangle[tileMap.Count];
            TileBoundingBoxes = new List<Rectangle>();
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = tileDict[tileMap[i]];
                locations[i] = new Rectangle((i % numCols) * 32, (i / numRows) * 32, 32, 32);

                // Construct bounding box(es) appropriate to the tile and position, and add them
                // to the Level's list
                if (tileBBs.ContainsKey(tileMap[i]))
                {
                    foreach (Rectangle bb in tileBBs[tileMap[i]])
                    {
                        Rectangle boundingBox = new Rectangle(
                            (i % numCols * 32) + (bb.X * scale), 
                            (i / numRows * 32) + (bb.Y * scale), 
                            bb.Width * scale, 
                            bb.Height * scale);
                        TileBoundingBoxes.Add(boundingBox);
                    }
                }
            }

            Hero = hero;

            /* For all the other initialisation, call Build() */
            Build();

        }

        // Build the level.
        public void Build()
        {
            // Initialise the props
            props = new List<Prop>();
            for (int i = 0; i < objectMap.Count; i++)
            {
                if (objectMap[i] != "null")
                {
                    // Build a new prop based off the provided template
                    Prop template = objectDict[objectMap[i]];
                    if (template is DestructibleProp)
                    {
                        var destrProp = ((DestructibleProp)template).Copy();
                        destrProp.Position =
                            new Vector2(i % numCols * 32, i / numRows * 32);
                        destrProp.Bounds = new Rectangle(
                                (i % numCols * 32) + (template.Bounds.X * scale),
                                (i / numRows * 32) + (template.Bounds.Y * scale),
                                template.Bounds.Width * scale,
                                template.Bounds.Height * scale);

                        props.Add(destrProp);
                    }
                    else
                    {
                        var prop = template.Copy();
                        prop.Position =
                            new Vector2(i % numCols * 32, i / numRows * 32);
                        prop.Bounds = new Rectangle(
                                (i % numCols * 32) + (template.Bounds.X * scale),
                                (i / numRows * 32) + (template.Bounds.Y * scale),
                                template.Bounds.Width * scale,
                                template.Bounds.Height * scale);

                        props.Add(prop);
                    }
                }
            }

            // Initialise the mobiles
            mobiles = new List<Mobile>();
            for (int i = 0; i < mobileMap.Count; i++)
            {
                if (mobileMap[i] != "null")
                {
                    // Build a new mobile based off the provided template
                    Mobile template = mobileDict[mobileMap[i]];
                    if (template is Monster)
                    {
                        var monster = template.Copy();
                        monster.Position = new Vector2(i % numCols * 32, i / numRows * 32);
                        monster.Bounds = new Rectangle(
                            (i % numCols * 32) + (template.Bounds.X * scale),
                            (i / numRows * 32) + (template.Bounds.Y * scale),
                            template.Bounds.Width * scale,
                            template.Bounds.Height * scale);
                        mobiles.Add(monster);
                    }
                    else
                    {
                        // Nothing else at the moment...
                    }
                }
            }

            // Initialise the zoneTriggers
            zoneTriggers = new List<ZoneTrigger>();
            for (int i = 0; i < zoneTriggerMap.Count; i++)
            {
                if (zoneTriggerMap[i] != "null")
                {
                    // Build the ZoneTrigger from the data in the string
                    string[] data = zoneTriggerMap[i].Split('-');
                    var bounds = new Rectangle(
                        i % numCols * 32,
                        i / numRows * 32,
                        32,
                        32);
                    var zt = new ZoneTrigger(data[0], int.Parse(data[1]), int.Parse(data[2]),
                        int.Parse(data[3]), int.Parse(data[4]), bounds);
                    zoneTriggers.Add(zt);
                }
            }

            // Initialise the interactibles
            interactibles = new List<InteractibleProp>();
            for (int i = 0; i < interactibleMap.Count; i++)
            {
                if (interactibleMap[i] != "null")
                {
                    // Build the interactible using the provided template
                    string[] data = interactibleMap[i].Split('|');
                    var template = interactibleDict[data[0]];
                    var interactible = template.Copy();
                    interactible.Position = new Vector2(i % numCols * 32, i / numRows * 32);
                    interactible.Bounds = new Rectangle(
                        (i % numCols * 32) + (template.Bounds.X * scale),
                        (i / numRows * 32) + (template.Bounds.Y * scale),
                        template.Bounds.Width * scale,
                        template.Bounds.Height * scale);
                    // Adapt the template using the information from the string
                    interactible.DialogText = data[1];
                    props.Add(interactible);
                }
            }

            // Initialise additional lists
            collidableProps = props.Where(p => p.IsSolid).ToList();

            destructibles = props.Where(p => p is DestructibleProp)
                .Select(wo => (IDestructible)wo).ToList();

            interactibles = props.Where(p => p is InteractibleProp)
                .Select(p => p as InteractibleProp).ToList();

            mobiles.ForEach(m =>
            {
                if (m is Monster)
                {
                    destructibles.Add((IDestructible)m);
                }
            });

            drawables = new List<WorldObject>(props);
            mobiles.Add(Hero);
            mobiles.ForEach(mob => drawables.Add(mob));
        }

        public void OnLoad()
        {
            ScreenManager.PlaySong(zoneName);
        }

        public void Update(GameTime gt, KeyboardState ks)
        {
            //Console.WriteLine(
            //    LevelManager.Zones.First(zone => zone.Value == LevelManager.CurrentZone).Key);

            // Toggle bounding boxes
            if (ks.IsKeyDown(Keys.B) && prevKS.IsKeyUp(Keys.B))
            {
                showBoundingBoxes = !showBoundingBoxes;
            }
            prevKS = ks;

            props.ForEach(wo => wo.Update(gt));
            mobiles.ForEach(mob => mob.Update(gt, ks)); // includes player

            LevelManager.AddCollectibles();
            LevelManager.RemoveDestructibles();

            // Sort the drawables. Nonsolid props should always be below solid ones.
            // This is probably pretty inefficient, since most drawables never change 
            // their order. 

            //drawables.Sort((d1, d2) =>
            //{
            //    if ((!d1.IsSolid && !d2.IsSolid) || (d1.IsSolid && d2.IsSolid))
            //    {
            //        if (d1.Bounds.Y == d2.Bounds.Y)
            //        {
            //            return d1.Bounds.X.CompareTo(d2.Bounds.X);
            //        }
            //        else return d1.Bounds.Y.CompareTo(d2.Bounds.Y);
            //    }
            //    else if (!d1.IsSolid) return -1;
            //    else return 1;
            //});

            drawables.Sort((d1, d2) =>
            {
                if (d1.Bounds.Y == d2.Bounds.Y)
                {
                    return d1.Bounds.X.CompareTo(d2.Bounds.X);
                }
                else return d1.Bounds.Y.CompareTo(d2.Bounds.Y);
            });

            // Level is responsible for updating HUD...
            LevelManager.Hud.Update(gt);
        }

        // This is called from ScreenManager, when CurrentState = Level
        // So Level can begin its own spriteBatch here
        // Note that we're making Level responsible for drawing HUD...
        public void Draw(SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.Deferred, null, null, null, null, null, 
                LevelManager.DefaultMatrix);
            for (int i = 0; i < tiles.Length; i++)
            {
                sb.Draw(tiles[i], locations[i], lightColor);
            }
            foreach (WorldObject drawable in drawables)
            {
                drawable.Draw(sb, lightColor);
            }
            // Draw bounding boxes, if applicable
            if (showBoundingBoxes)
            {
                drawables.ForEach(wo => LineBatch.drawLineRectangle(sb, wo.Bounds, Color.Red));
            }
            sb.End();
            sb.Begin();
            LevelManager.Hud.Draw(sb, lightColor);
            sb.End();
        }

        // This is called from FadeTransition
        public void Draw(SpriteBatch sb, Color screenFilter)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                sb.Draw(tiles[i], locations[i], screenFilter);
            }
            foreach (WorldObject drawable in drawables)
            {
                drawable.Draw(sb, screenFilter);
            }

            // Test bounding boxes
            //drawables.ForEach(d => sb.Draw(testing, d.Bounds, Color.Red));

        }

        // Test to get the level transition looking nicer
        public void DrawWithoutHero(SpriteBatch sb, Color screenFilter)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                sb.Draw(tiles[i], locations[i], screenFilter);
            }
            foreach (WorldObject drawable in drawables)
            {
                if (!(drawable is Player))
                {
                    drawable.Draw(sb, screenFilter);
                }
            }
        }
        
    }
}
