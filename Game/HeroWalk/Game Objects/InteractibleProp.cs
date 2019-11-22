using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using System.Collections.Generic;

namespace HeroWalk
{
    public delegate void InteractScript(IInteractible interactible);

    public interface IInteractible
    {
        string DialogText { get; set; }
        InteractScript OnInteract { get; set; }
        List<Direction> InteractibleDirections { get; set; }
        SoundEffect InteractionSound { get; set; }
    }

    public class InteractibleProp : Prop, IInteractible
    {
        public string DialogText { get; set; }
        public InteractScript OnInteract { get; set; }
        public List<Direction> InteractibleDirections { get; set; }
        public SoundEffect InteractionSound { get; set; }

        // The 'interaction point', relative to the top-left corner
        public Vector2 InteractionPoint { get; set; }

        public InteractibleProp(Vector2 position, Sprite sprite, Rectangle bounds,
            bool isSolid, string dialogText, InteractScript onInteract,
            List<Direction> interactibleDirections, Vector2 interactionPoint,
            float animsPerSecond = 1, SoundEffect interactionSound = null) : 
            base(position, sprite, bounds, isSolid, animsPerSecond)
        {
            DialogText = dialogText;
            OnInteract = onInteract;
            InteractibleDirections = interactibleDirections;
            InteractionSound = interactionSound;
            InteractionPoint = interactionPoint;
        }

        public new InteractibleProp Copy()
        {
            return new InteractibleProp(Position, Sprite, Bounds, IsSolid, 
                DialogText, OnInteract, InteractibleDirections, InteractionPoint, AnimsPerSecond, 
                InteractionSound);
        }
    }
}
