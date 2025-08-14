using System;
using System.Collections.Generic;
using Eflatun.SceneReference;
using System.Linq;

namespace Systems.SceneManagment
{
    [Serializable]
    public class SceneGroup
    {
        public string GroupName = "New Scene Group";
        public List<SceneData> Scenes;

        /// <summary>
        /// Permite buscar la primera escena en el grupo que coincida con el tipo (Scenetype) y devuelve su nombre.
        /// </summary>
        /// <param name="sceneType"></param>
        /// <returns></returns>
        public string FindSceneNameByType(SceneType sceneType)
        {
            return Scenes.FirstOrDefault(scene => scene.SceneType == sceneType)?.Reference.Name;
        }
    }
    
    [Serializable]
    public class SceneData
    {
        public SceneReference Reference;
        public string Name => Reference.Name;
        public SceneType SceneType;
    }
    public enum SceneType { ActiveScene, MainMenu, UserInterface, HUD, Cinematic, Environment, Tooling }
}
