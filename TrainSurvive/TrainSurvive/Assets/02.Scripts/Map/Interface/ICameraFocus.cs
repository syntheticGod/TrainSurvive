/*
 * 描述：控制摄像机移动的接口
 * 作者：项叶盛
 * 创建时间：2018/10/31 2:10:34
 * 版本：v0.1
 */
using UnityEngine;
namespace WorldMap
{
    public interface ICameraFocus
    {
        void focusOnce(Transform t);
        void focusLock(Transform t);
    }
}