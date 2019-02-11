using System.Collections.Generic;
using System.IO;

namespace Szark
{
    public class SpriteCollection
    {
        private Dictionary<string, Sprite> sprites;

        /// <summary>
        /// Creates a collection of sprites given
        /// a directory on the computer.
        /// </summary>
        public SpriteCollection(string directory)
        {
            sprites = new Dictionary<string, Sprite>();
            var paths = Directory.GetFiles(directory);

            foreach (var path in paths)
            {
                if (path.Contains(".png") || path.Contains(".jpg"))
                {
                    var nameSplit = path.Split('.');
                    var directorySplit = nameSplit[0].Split('\\');
                    var spriteName = directorySplit[directorySplit.Length - 1];
                    sprites.Add(spriteName, new Sprite(path));
                }
            }
        }

        /// <summary>
        /// Adds a sprite to the collection with a name
        /// </summary>
        /// <param name="name">The key</param>
        public void AddSprite(string name, Sprite sprite)
        {
            if (sprite == null) return;
            sprites.Add(name, sprite);
        }

        /// <summary>
        /// Removes a sprite from the collection given
        /// a name / key.
        /// </summary>
        public bool RemoveSprite(string name)
        {
            if (sprites.ContainsKey(name))
            {
                sprites.Remove(name);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gives a sprite given a name
        /// </summary>
        public Sprite GetSprite(string name)
        {
            if (sprites.ContainsKey(name))
                return sprites[name];
            return null;
        }
    }
}
