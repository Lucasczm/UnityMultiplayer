using UnityEngine;
namespace Game
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        public static GameManager instance;
        void Awake()
        {
            if (instance != null) return;
            instance = this;
        }
        #endregion
    }
}