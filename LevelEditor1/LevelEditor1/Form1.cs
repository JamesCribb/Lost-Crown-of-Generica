using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace LevelEditor1
{
    public partial class Form1 : Form
    {
        private const int numRows = 20;
        private const int numCols = 20;

        // Holds the tiles read in from file
        private Dictionary<string, Bitmap> tiles;
        // Holds a representation of the level in terms of the name of each tile
        private string[][] tileMap;
        // Holds the objects read in from file
        private Dictionary<string, Bitmap> props;
        // Representation of the object at each grid location, if any
        private string[][] propMap;
        // Holds the mobiles read in from file
        private Dictionary<string, Bitmap> mobiles;
        // Representation of the mobile at each grid location, if any
        private string[][] mobileMap;
        // Holds the interactibles read in from file
        private Dictionary<string, Bitmap> interactibles;
        // Representation of the interactible at each grid location, if any
        private string[][] interactibleMap;
        // Representation of the zoneTrigger at each grid location, if any
        private string[][] zoneTriggerMap;

        // For drawing the grid
        Pen pen = new Pen(Color.Black);

        Bitmap leftMouseTile;
        Bitmap rightMouseTile;
        Bitmap leftMouseObject;
        Bitmap leftMouseMobile;
        Bitmap leftMouseInteractible;

        Bitmap eraserImage;
        Bitmap zoneTriggerImage;
        Bitmap zoneTriggerSelectedImage;

        int selectedX = -1;
        int selectedY = -1;

        // Holds the name of the folder from which the current level has been loaded
        private string pathName;

        public Form1()
        {
            InitializeComponent();

            tiles = new Dictionary<string, Bitmap>();
            props = new Dictionary<string, Bitmap>();
            mobiles = new Dictionary<string, Bitmap>();
            interactibles = new Dictionary<string, Bitmap>();

            TileListFLP.AutoScroll = true;
            ObjectListFLP.AutoScroll = true;
            MobileListFLP.AutoScroll = true;
            InteractibleListFLP.AutoScroll = true;

            AddFilesToDictAndPanel("Tiles", tiles, TileListFLP);
            AddFilesToDictAndPanel("Objects", props, ObjectListFLP);
            AddFilesToDictAndPanel("Mobiles", mobiles, MobileListFLP);
            AddFilesToDictAndPanel("Interactibles", interactibles, InteractibleListFLP);
            Console.WriteLine(interactibles.Count);

            AssociateClickHandlerWithFLP(TileListFLP, SelectTile);
            AssociateClickHandlerWithFLP(ObjectListFLP, SelectObject);
            AssociateClickHandlerWithFLP(MobileListFLP, SelectMobile);
            AssociateClickHandlerWithFLP(InteractibleListFLP, SelectInteractible);

            // Add a method to save zoneTrigger information
            zoneTriggerPanel.GetSaveButton().MouseClick += SaveZoneTriggerInfo;

            // Add a method to save interactible information
            InteractiblePanel.GetSaveButton().MouseClick += SaveInteractibleInfo;

            // Initialise the maps
            tileMap = InitialiseMap("Grass1");
            propMap = InitialiseMap("null");
            mobileMap = InitialiseMap("null");
            zoneTriggerMap = InitialiseMap("null");
            interactibleMap = InitialiseMap("null");

            LayerSelectCB.SelectedValueChanged += ChangeLayer;
            LayerSelectCB.SelectedIndex = 0; // Tile Layer

            leftMouseTile = tiles["Grass1"];
            rightMouseTile = tiles["Grass1"];
            LmbPB.Image = leftMouseTile;
            RmbPB.Image = rightMouseTile;

            leftMouseObject = props[props.Keys.ToList()[0]];
            leftMouseMobile = mobiles[mobiles.Keys.ToList()[0]];
            leftMouseInteractible = interactibles[interactibles.Keys.ToList()[0]];

            eraserImage = new Bitmap("eraser.png");
            zoneTriggerImage = new Bitmap("zoneTrigger.png");
            zoneTriggerSelectedImage = new Bitmap("zoneTriggerSelected.png");

            // Use reflection to set double buffering property of panel to true
            // Courtesy of StackOverflow
            PropertyInfo propertyInfo = typeof(Control).GetProperty(
                "DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            propertyInfo.SetValue(LevelEditorPanel, true, null);

            ObjectListFLP.Hide();
            MobileListFLP.Hide();
            zoneTriggerPanel.Hide();
            InteractibleListFLP.Hide();
            InteractiblePanel.Hide();

            LevelEditorPanel.Refresh();
        }

        private void AddFilesToDictAndPanel(string directory, 
            Dictionary<string, Bitmap> dictionary, FlowLayoutPanel panel)
        {
            var files = Directory.EnumerateFiles(directory).ToArray();
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                string fileName = file.Split('\\')[1].Split('.')[0];
                
                // Try to avoid the weird usb-related error
                if (file == @"Objects\Thumbs.db") continue;

                Bitmap raw = new Bitmap(file);
                Bitmap scaled = new Bitmap(raw, new Size(raw.Width * 2, raw.Height * 2));
                scaled.MakeTransparent(Color.White);
                dictionary.Add(fileName, scaled);
                var pictureBox = new PictureBox();
                pictureBox.Image = scaled;
                pictureBox.Width = scaled.Width;
                pictureBox.Height = scaled.Height;
                panel.Controls.Add(pictureBox);
            }
        }

        private string[][] InitialiseMap(string defaultValue)
        {
            string[][] map = new string[numRows][];
            for (int i = 0; i < map.Length; i++)
            {
                map[i] = new string[numCols];
                for (int j = 0; j < map[i].Length; j++)
                {
                    map[i][j] = defaultValue;
                }
            }
            return map;
        }

        private void AssociateClickHandlerWithFLP(FlowLayoutPanel panel, 
            MouseEventHandler selectFunction)
        {
            foreach (Control control in panel.Controls)
            {
                control.MouseClick += selectFunction;
            }
        }

        public void LevelEditorPanel_Paint(object sender, PaintEventArgs e)
        {
            DrawFromMap(tileMap, tiles, e);
            DrawFromMap(propMap, props, e);
            DrawFromMap(mobileMap, mobiles, e);
            DrawFromMap(zoneTriggerMap, zoneTriggerImage, e);
            DrawFromMap(interactibleMap, interactibles, e);

            // Draw the selected zoneTrigger, if applicable
            if (selectedX != -1 && selectedY != -1 && LayerSelectCB.SelectedItem.ToString() == "ZoneTrigger Layer") 
            {
                Bitmap bmp = zoneTriggerSelectedImage;
                e.Graphics.DrawImage(bmp, selectedX * 32, selectedY * 32, bmp.Width, bmp.Height);
            }

            // Draw a grid
            for (int i = 0; i <= 640; i += 32)
            {
                e.Graphics.DrawLine(pen, 0, i, LevelEditorPanel.Width, i);
                e.Graphics.DrawLine(pen, i, 0, i, LevelEditorPanel.Height);
            }
        }

        private void DrawFromMap(string[][] map, Dictionary<string, Bitmap> dictionary,
            PaintEventArgs e)
        {
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    if (map[i][j] != "null")
                    {
                        // This should keep it from crashing with interactibles...
                        string key = map[i][j].Split('|')[0];

                        //// Old version
                        //Bitmap bmp = dictionary[map[i][j]];

                        Bitmap bmp = dictionary[key];

                        e.Graphics.DrawImage(bmp, i * 32, j * 32, bmp.Width, bmp.Height);
                    }
                }
            }
        }

        private void DrawFromMap(string[][] map, Bitmap bmp, PaintEventArgs e)
        {
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    if (map[i][j] != "null")
                    {
                        e.Graphics.DrawImage(bmp, i * 32, j * 32, bmp.Width, bmp.Height);
                    }
                }
            }
        }

        private void LevelEditorPanel_MouseClick(object sender, MouseEventArgs e)
        {
            UpdateLevel(e);
        }

        private void LevelEditorPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseButtons == MouseButtons.Left || MouseButtons == MouseButtons.Right)
            {
                UpdateLevel(e);
            }
        }

        // Update the relevant map with the new value, then repaint the panel
        private void UpdateLevel(MouseEventArgs e)
        {
            int tileX = e.Location.X / 32;
            int tileY = e.Location.Y / 32;

            // Stop the program from crashing if mouse is held down outside panel
            if (tileX < 0 || tileX >= numCols ||
                tileY < 0 || tileY >= numRows)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                if (LayerSelectCB.SelectedItem.ToString() == "Tile Layer")
                {
                    // Mildly dubious solution to get a key from a value
                    // courtesy of StackOverflow
                    tileMap[tileX][tileY] = tiles.First(t => t.Value == leftMouseTile).Key;
                }
                else if (LayerSelectCB.SelectedItem.ToString() == "Prop Layer")
                {
                    propMap[tileX][tileY] = props
                        .First(obj => obj.Value == leftMouseObject).Key;
                }
                else if (LayerSelectCB.SelectedItem.ToString() == "Mobile Layer")
                {
                    mobileMap[tileX][tileY] = mobiles
                        .First(mob => mob.Value == leftMouseMobile).Key;
                }
                else if (LayerSelectCB.SelectedItem.ToString() == "ZoneTrigger Layer")
                {
                    if (zoneTriggerMap[tileX][tileY] != "null")
                    {
                        // 'Select' the relevant zoneTrigger and load its info
                        selectedX = tileX;
                        selectedY = tileY;
                        LoadZoneTriggerInfo();
                    }
                    else
                    {
                        // This is a placeholder; the proper values are entered via the panel
                        zoneTriggerMap[tileX][tileY] = "null-0-0-0-0";
                    }
                }
                else if (LayerSelectCB.SelectedItem.ToString() == "Interactible Layer")
                {
                    if (interactibleMap[tileX][tileY] != "null")
                    {
                        //Console.WriteLine("Selecting the interactible from the map");
                        // Select the relevant interactible and load its info
                        selectedX = tileX;
                        selectedY = tileY;
                        LoadInteractibleInfo();
                    }
                    else
                    {
                        //Console.WriteLine("Placing the interactible in the map");
                        string fileName = interactibles.First(i => i.Value == leftMouseInteractible).Key;
                        interactibleMap[tileX][tileY] = $"{fileName}|{"hello world"}";
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (LayerSelectCB.SelectedItem.ToString() == "Tile Layer")
                {
                    tileMap[tileX][tileY] = tiles.First(t => t.Value == rightMouseTile).Key;
                    //tileMap[tileX][tileY] = tiles.IndexOf(rightMouseTile);
                }
                else if (LayerSelectCB.SelectedItem.ToString() == "Prop Layer")
                {
                    // 'Erase' the object
                    propMap[tileX][tileY] = "null";
                }
                else if (LayerSelectCB.SelectedItem.ToString() == "Mobile Layer")
                {
                    // Erase the mobile
                    mobileMap[tileX][tileY] = "null";
                }
                else if (LayerSelectCB.SelectedItem.ToString() == "ZoneTrigger Layer")
                {
                    // Erase the zoneTrigger
                    zoneTriggerMap[tileX][tileY] = "null";
                }
                else if (LayerSelectCB.SelectedItem.ToString() == "Interactible Layer")
                {
                    // Erase the interactible
                    interactibleMap[tileX][tileY] = "null";
                }
            }
            LevelEditorPanel.Refresh();
        }

        // Change the tile drawn by the left or right mouse button
        // TODO: Refactor these into a single SelectX method?
        private void SelectTile(object sender, MouseEventArgs e)
        {
            if (LayerSelectCB.SelectedItem.ToString() == "Tile Layer")
            {
                string fileName = 
                    tiles.First(t => t.Value == (Bitmap)((PictureBox)sender).Image).Key;
                //int index = tiles.IndexOf((Bitmap)((PictureBox)sender).Image);

                if (e.Button == MouseButtons.Left)
                {
                    leftMouseTile = tiles[fileName];
                    LmbPB.Image = tiles[fileName];
                }
                else if (e.Button == MouseButtons.Right)
                {
                    rightMouseTile = tiles[fileName];
                    RmbPB.Image = tiles[fileName];
                }
            }
        }

        // Change the object drawn by the left mouse button
        private void SelectObject(object sender, MouseEventArgs e)
        {
            if (LayerSelectCB.SelectedItem.ToString() == "Prop Layer")
            {
                string fileName = props.First(
                    obj => obj.Value == (Bitmap)((PictureBox)sender).Image).Key;
                //int index = objects.IndexOf((Bitmap)((PictureBox)sender).Image);
                if (e.Button == MouseButtons.Left)
                {
                    leftMouseObject = props[fileName];
                    // Scale the image to fit in the mouse icon pictureBox
                    LmbPB.Image = new Bitmap(props[fileName], new Size(32, 32));
                }
            }
        }

        // Change the mobile drawn by the left mouse button
        private void SelectMobile(object sender, MouseEventArgs e)
        {
            if (LayerSelectCB.SelectedItem.ToString() == "Mobile Layer")
            {
                string fileName = mobiles.First(
                    mob => mob.Value == (Bitmap)((PictureBox)sender).Image).Key;
                //int index = mobiles.IndexOf((Bitmap)((PictureBox)sender).Image);
                if (e.Button == MouseButtons.Left)
                {
                    leftMouseMobile = mobiles[fileName];
                    // Scale the image to fit in the mouse icon pictureBox
                    LmbPB.Image = new Bitmap(mobiles[fileName], new Size(32, 32));
                }
            }
        }

        // Change the interactible drawn by the left mouse button
        private void SelectInteractible(object sender, MouseEventArgs e)
        {
            if (LayerSelectCB.SelectedItem.ToString() == "Interactible Layer")
            {
                string fileName = interactibles.First(
                    i => i.Value == (Bitmap)((PictureBox)sender).Image).Key;
                if (e.Button == MouseButtons.Left)
                {
                    leftMouseInteractible = interactibles[fileName];
                    // Scale the image to fit the pictureBox
                    LmbPB.Image = new Bitmap(interactibles[fileName], new Size(32, 32));
                }
            }
        }

        // Save zoneTrigger information entered via the panel
        private void SaveZoneTriggerInfo(object sender, MouseEventArgs e)
        {
            if (selectedX != -1 && selectedY != -1)
            {
                string zone = zoneTriggerPanel.GetZoneTB().Text;
                string levelX = zoneTriggerPanel.GetLevelXNUD().Value.ToString();
                string levelY = zoneTriggerPanel.GetLevelYNUD().Value.ToString();
                string playerX = zoneTriggerPanel.GetPlayerXNUD().Value.ToString();
                string playerY = zoneTriggerPanel.GetPlayerYNUD().Value.ToString();

                string zoneTriggerInfo = $"{zone}-{levelX}-{levelY}-{playerX}-{playerY}";
                zoneTriggerMap[selectedX][selectedY] = zoneTriggerInfo;
            }
        }

        private void LoadZoneTriggerInfo()
        {
            string[] zoneTriggerInfo = zoneTriggerMap[selectedX][selectedY].Split('-');
            zoneTriggerPanel.GetZoneTB().Text = zoneTriggerInfo[0];
            zoneTriggerPanel.GetLevelXNUD().Value = decimal.Parse(zoneTriggerInfo[1]);
            zoneTriggerPanel.GetLevelYNUD().Value = decimal.Parse(zoneTriggerInfo[2]);
            zoneTriggerPanel.GetPlayerXNUD().Value = decimal.Parse(zoneTriggerInfo[3]);
            zoneTriggerPanel.GetPlayerYNUD().Value = decimal.Parse(zoneTriggerInfo[4]);
        }

        private void SaveInteractibleInfo(object sender, MouseEventArgs e)
        {
            if (selectedX != -1 && selectedY != -1)
            {
                string fileName = InteractiblePanel.GetFileNameLabel().Text;
                string text = InteractiblePanel.GetInteractionTextRTB().Text;

                string interactibleInfo = $"{fileName}|{text}";
                interactibleMap[selectedX][selectedY] = interactibleInfo;
            }
        }

        private void LoadInteractibleInfo()
        {
            string[] interactibleInfo = interactibleMap[selectedX][selectedY].Split('|');
            InteractiblePanel.GetFileNameLabel().Text = interactibleInfo[0];
            InteractiblePanel.GetInteractionTextRTB().Text = interactibleInfo[1];
        }

        // When the layer is changed, display the appropriate elements in the mouse 
        // picture boxes. 
        private void ChangeLayer(object sender, EventArgs e)
        {
            if (LayerSelectCB.SelectedItem.ToString() == "Tile Layer")
            {
                LmbPB.Image = leftMouseTile;
                RmbPB.Image = rightMouseTile;

                TileListFLP.Show();
                ObjectListFLP.Hide();
                MobileListFLP.Hide();
                zoneTriggerPanel.Hide();
                InteractibleListFLP.Hide();
                InteractiblePanel.Hide();
            }
            else if (LayerSelectCB.SelectedItem.ToString() == "Prop Layer")
            {
                LmbPB.Image = leftMouseObject;
                RmbPB.Image = eraserImage;

                TileListFLP.Hide();
                ObjectListFLP.Show();
                MobileListFLP.Hide();
                zoneTriggerPanel.Hide();
                InteractibleListFLP.Hide();
                InteractiblePanel.Hide();
            }
            else if (LayerSelectCB.SelectedItem.ToString() == "Mobile Layer")
            {
                LmbPB.Image = leftMouseMobile;
                RmbPB.Image = eraserImage;

                TileListFLP.Hide();
                ObjectListFLP.Hide();
                MobileListFLP.Show();
                zoneTriggerPanel.Hide();
                InteractibleListFLP.Hide();
                InteractiblePanel.Hide();
            }
            else if (LayerSelectCB.SelectedItem.ToString() == "ZoneTrigger Layer")
            {
                LmbPB.Image = zoneTriggerImage;
                RmbPB.Image = eraserImage;

                TileListFLP.Hide();
                ObjectListFLP.Hide();
                MobileListFLP.Hide();
                zoneTriggerPanel.Show();
                InteractibleListFLP.Hide();
                InteractiblePanel.Hide();
            }
            else if (LayerSelectCB.SelectedItem.ToString() == "Interactible Layer")
            {
                LmbPB.Image = leftMouseInteractible;
                RmbPB.Image = eraserImage;

                TileListFLP.Hide();
                ObjectListFLP.Hide();
                MobileListFLP.Hide();
                zoneTriggerPanel.Hide();
                InteractibleListFLP.Show();
                InteractiblePanel.Show();
            }
        }

        /* Create a new folder with a timestamp, then write the tile and object maps
           to separate text files. */
        private void SaveLevel(string path)
        {
            // Overwrite existing folder, or create a new one
            if (path == null)
            {
                string folderName = DateTime.Now.Ticks.ToString();
                path = Directory.CreateDirectory($@"Output/{folderName}").FullName;
            }

            SaveMapToFile(path, "tileMap", tileMap);
            SaveMapToFile(path, "objectMap", propMap);
            SaveMapToFile(path, "mobileMap", mobileMap);
            SaveMapToFile(path, "zoneTriggerMap", zoneTriggerMap);
            SaveMapToFile(path, "interactibleMap", interactibleMap);
        }

        private void SaveMapToFile(string pathName, string fileName, string[][] map)
        {
            using (var writer = new StreamWriter($@"{pathName}\{fileName}.txt"))
            {
                for (int i = 0; i < map.Length; i++)
                {
                    for (int j = 0; j < map[i].Length; j++)
                    {
                        writer.WriteLine(map[j][i]);
                    }
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveLevel(null);
        }

        private void OverwriteSaveButton_Click(object sender, EventArgs e)
        {
            SaveLevel(pathName);
        }

        private void LoadFileButton_Click(object sender, EventArgs e)
        {
            var ofbd = new Ookii.Dialogs.WinForms.VistaFolderBrowserDialog();

            if (ofbd.ShowDialog() == DialogResult.OK)
            {
                string[] files = Directory.GetFiles(ofbd.SelectedPath);

                // Alphabetical order!

                // Old files without interactible layer
                if (files.Length == 4)
                {
                    LoadMapFromFile(files[0], mobileMap, "null");
                    LoadMapFromFile(files[1], propMap, "null");
                    LoadMapFromFile(files[2], tileMap, "Grass1");
                    LoadMapFromFile(files[3], zoneTriggerMap, "null");
                }
                // Up to date files
                else
                {
                    LoadMapFromFile(files[0], interactibleMap, "null");
                    LoadMapFromFile(files[1], mobileMap, "null");
                    LoadMapFromFile(files[2], propMap, "null");
                    LoadMapFromFile(files[3], tileMap, "Grass1");
                    LoadMapFromFile(files[4], zoneTriggerMap, "null");
                }

                pathName = ofbd.SelectedPath;
                OverwriteSaveButton.Enabled = true;
                LevelEditorPanel.Refresh();
            }
        }

        private void LoadMapFromFile(string fileName, string[][] map, string defaultValue)
        {
            var tempList = new List<string>();
            using (var reader = new StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    tempList.Add(line);
                }
            }
            int listCounter = 0;
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    // If we've deleted the object in question, we don't want the program
                    // to crash
                    if (tempList[listCounter] != null)
                    {
                        map[j][i] = tempList[listCounter];
                    }
                    else
                    {
                        map[j][i] = defaultValue;
                    }
                    listCounter++;
                }
            }
        }

    }
}
