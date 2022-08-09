// TODO: Clean

/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.LevelLoader;

namespace Platformer.LevelLoader {

    ///<summary>
    ///
    ///<summary>
    public class Zones {

        public static string ForgottensCave = "The Forgotten's Cave";
        public static string LoversForest = "Lovers Forest";
        public static string AncientAbyss = "Ancient Abyss";

        public static string Get(Zone zone) {
            switch (zone) {
                case Zone.ForgottensCave:
                    return ForgottensCave;
                case Zone.LoversForest:
                    return LoversForest;
                case Zone.AncientAbyss:
                    return AncientAbyss;
                default:
                    return "";
            }
        }

    }

    public enum Zone {
        ForgottensCave,
        LoversForest,
        AncientAbyss
    }

}

