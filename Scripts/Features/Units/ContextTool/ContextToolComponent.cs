using UnityEngine;

namespace Client
{
    public struct ContextToolComponent
    {
        /// <summary>
        /// 0-pickaxe, 1-sword, 2-bow, 3-empty
        /// </summary>
        public enum Tool { pickaxe, sword, bow, empty };

        public Tool CurrentActiveTool;

        public GameObject[] ToolsPool;
    }
}