using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hyperspace
{
    public class Utils : MonoBehaviour
    {
        public static string GetRoomCode(int levelIndex, string roomCode)
        {
            return string.Format("Room{0}-{1}", levelIndex, roomCode);
        }
    }
}
