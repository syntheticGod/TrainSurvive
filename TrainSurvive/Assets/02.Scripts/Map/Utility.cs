using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMap {
    //功能类函数
    public static class Utility {
        public static void Swap<T>(ref T my, ref T other) {
            T temp = my;
            my = other;
            other = temp;
        }
    }
}
