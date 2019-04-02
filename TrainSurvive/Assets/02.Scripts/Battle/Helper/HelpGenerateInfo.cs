/*
 * 描述：产生浮动信息
 * 作者：王安鑫
 * 创建时间：2019/1/5 16:19:01
 * 版本：v0.7
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WorldBattle {
    public class HelpGenerateInfo : MonoBehaviour {

        //文本大小
        public const int fontSize = 5;
        public const float textCharacterSize = 0.1f;
        //上浮距离
        public Vector3 offset = new Vector3(0, 0.5f, 0.0f);
        //上浮的高度
        public const float height = 1.5f;
        //上浮时间
        public const float floatTime = 1.5f;

        /// <summary>
        /// 产生浮动信息
        /// </summary>
        /// <param name="pos">产生浮动信息的位置</param>
        /// <param name="text">产生浮动信息的文字</param>
        public static void generateInfo(BattleActor battleActor, string text) {
            //创建一个空对象
            GameObject textObject = new GameObject();

            //设置对象的位置
            textObject.transform.position = battleActor.playerPrefab.transform.position;
            //将其赋予文字属性
            TextMesh textMesh = textObject.AddComponent<TextMesh>();

            //设置文字信息
            textMesh.text = text;
            //设置居中
            textMesh.alignment = TextAlignment.Center;
            textMesh.anchor = TextAnchor.MiddleCenter;
            //设置字体大小
            textMesh.fontSize = 2*(int)(fontSize / textCharacterSize);
            textMesh.characterSize = textCharacterSize;
            //设置字体加粗
            textMesh.fontStyle = FontStyle.Bold;
            //设置字体颜色
            textMesh.color = Color.red;

            //开启协程向上浮动并淡化
            battleActor.StartCoroutine(textfloatUp(textObject));
        }

        static IEnumerator textfloatUp(GameObject textObject) {
            //获取文本对象
            TextMesh textMesh = textObject.GetComponent<TextMesh>();
            
            //获取其文本的颜色
            Color color = textMesh.color;
            //获取对象的位置
            Vector3 pos = textObject.transform.position;
            //开始计时
            float curTime = 0.0f;

            while (curTime < floatTime) {
                //增加时间
                curTime += Time.deltaTime;
                //向上浮动
                textObject.transform.position = pos + new Vector3(0.0f, curTime / floatTime * height, 0.0f);
                //淡化
                color.a = 1 - curTime / floatTime;
                textMesh.color = color;
                
                //返回
                yield return 0;
            }

            //销毁当前对象
            Destroy(textObject);
        }
    }
}

