using UnityEngine;
using VertexStudio.VirtualRealty;
using VertexStudio.Generic;

namespace VostokVR.Geo {

    public class LevelManagerIntro : LevelManager {

        public static LevelManagerIntro Instance { get; private set; }

        // Use this for initialization #####################################################################################################################################################################################################################
        protected override void Awake() {

            Instance = this;

            base.Awake();
        }

        // Use this for initialization #####################################################################################################################################################################################################################
        protected override void Start() {

            base.Start();

            ViveInteractionsManager.Instance.ShowPointers();

            //Resources.UnloadUnusedAssets();
        }

        // On application quit #############################################################################################################################################################################################################################
        protected override void OnApplicationQuit() {
 
            base.OnApplicationQuit();
        }
    }
}