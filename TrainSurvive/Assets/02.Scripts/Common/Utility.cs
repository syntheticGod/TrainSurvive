/*
 * 描述：工具类。包含数学、视图、矩阵
 * 作者：项叶盛
 * 创建时间：2018/11/12 21:46:39
 * 版本：v0.1
 */

using UnityEngine;
using UnityEngine.UI;

namespace TTT.Utility
{
    //功能类函数
    public static class MathTool
    {
        public static void Swap<T>(ref T my, ref T other)
        {
            T temp = my;
            my = other;
            other = temp;
        }
        private const float EPSILON_IN_VIEW = 1.0E-4F;
        /// <summary>
        /// 判断两个值是否在视觉上接近
        /// 差值在1.0x10^4以内的范围的视为相同
        /// </summary>
        /// <param name="a">向量1</param>
        /// <param name="b">向量2</param>
        /// <returns>
        /// TRUE：在视觉上接近
        /// FALSE：在视觉上不接近
        /// </returns>
        public static bool ApproximatelyInView(float a, float b)
        {
            return Mathf.Abs(a - b) < EPSILON_IN_VIEW;
        }
        /// <summary>
        /// 判断两个向量是否在视觉上接近
        /// x轴和y轴差值都在1.0x10^4以内的范围的视为相同
        /// </summary>
        /// <param name="a">向量1</param>
        /// <param name="b">向量2</param>
        /// <returns>
        /// TRUE：在视觉上接近
        /// FALSE：在视觉上不接近
        /// </returns>
        public static bool ApproximatelyInView(Vector2 a, Vector2 b)
        {
            return ApproximatelyInView(a.x, b.x) && ApproximatelyInView(a.y, b.y);
        }
        public static bool Approximately(Vector2 a, Vector2 b)
        {
            return Mathf.Approximately(a.x - b.x, 0.0F) && Mathf.Approximately(a.y - b.y, 0.0F);
        }
        /// <summary>
        /// 忽略3维向量的y轴
        /// (x,y,z) => (x,z)
        /// </summary>
        /// <param name="a"></param>
        /// <returns>2D向量</returns>
        public static Vector2 IgnoreY(Vector3 a)
        {
            return new Vector2
            {
                x = a.x,
                y = a.z
            };
        }
        public static Vector2 IgnoreZ(Vector3 a)
        {
            return new Vector2
            {
                x = a.x,
                y = a.y
            };
        }
        /// <summary>
        /// 添加y轴
        /// (x,z) => (x,y,z)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="y"></param>
        /// <returns>3D向量</returns>
        public static Vector3 AcceptY(Vector2 a, float y)
        {
            return new Vector3
            {
                x = a.x,
                y = y,
                z = a.y
            };
        }
        public static Vector3 AcceptZ(Vector2 a, float z)
        {
            return new Vector3
            {
                x = a.x,
                y = a.y,
                z = z
            };
        }
        /// <summary>
        /// 判断value是否在范围[edge1,edge2]或[edge2,edge1]之间
        /// </summary>
        /// <param name="edge1">范围边界1</param>
        /// <param name="edge2">范围边界2</param>
        /// <param name="value">值</param>
        /// <returns>
        /// TRUE：value在范围内
        /// FALSE：不在范围内
        /// </returns>
        public static bool IfBetweenBoth(float edge1, float edge2, float value)
        {
            if (Mathf.Approximately(edge1, value) || Mathf.Approximately(edge2, value))
                return true;
            return (value > edge1 && value < edge2) || (value > edge2 && value < edge1);
        }
        public static bool IfBetweenLeft(float edge1, float edge2, float value)
        {
            if (Mathf.Approximately(edge1, value))
                return true;
            if (Mathf.Approximately(edge2, value))
                return false;
            return (value > edge1 && value < edge2) || (value > edge2 && value < edge1);
        }
        public static bool IfBetweenBoth(int left, int right, int value)
        {
            return value >= left && value <= right;
        }

        private static System.Random rand = new System.Random();
        /// <summary>
        /// 生成 [0,maxValue) 范围内的整数
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int RandomInt(int maxValue)
        {
            return rand.Next(maxValue);
        }
        /// <summary>
        /// 生成[minValue,maxValue)范围的整数
        /// 如果minValue等于maxValue，则返回minValue
        /// maxValue必须大于等于minValue
        /// 否则异常 ArgumentOutOfRangeException
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int RandomRange(int minValue, int maxValue)
        {
            return rand.Next(minValue, maxValue);
        }
    }
    public static class CompTool
    {
        public static T ForceGetComponent<T>(GameObject gameo)
            where T : Component
        {
            T t = gameo.GetComponent<T>();
            return t == null ? gameo.AddComponent<T>() : t;
        }
        public static T ForceGetComponent<T>(Component component)
            where T : Component
        {
            T t = component.GetComponent<T>();
            if (t == null)
                t = component.gameObject.AddComponent<T>();
            return t;
        }
        public static T ForceGetComponent<T, N>(Component component)
            where T : Component
            where N : T
        {
            T t = component.GetComponent<T>();
            return t == null ? component.gameObject.AddComponent<N>() as T : t;
        }

    }
    public static class ViewTool
    {
        public static T ForceGetComponentInChildren<T>(GameObject gameo, string name, bool active = true)
            where T : Component
        {
            Transform transform = gameo.transform.Find(name);
            T t;
            if (transform == null)
            {
                transform = new GameObject(name).AddComponent<RectTransform>();
                transform.gameObject.SetActive(false);
                transform.gameObject.AddComponent<T>();
                transform.SetParent(gameo.GetComponent<Transform>());
                FullFillRectTransform(transform, Vector2.zero, Vector2.zero);
            }
            t = CompTool.ForceGetComponent<T>(transform);
            t.gameObject.SetActive(active);
            return t;
        }
        public static T ForceGetComponentInChildren<T>(Component comp, string name, bool active = true)
            where T : Component
        {
            return ForceGetComponentInChildren<T>(comp.gameObject, name, active);
        }
        public static void Anchor(Component comp, Vector2 anchorMin, Vector2 anchorMax)
        {
            RectTransform rect = comp.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
        public static void Anchor(Component comp, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            RectTransform rect = comp.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }
        public static void FullFillRectTransform(Component comp, Vector2 offsetMin, Vector2 offsetMax)
        {
            RectTransform rect = comp.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }
        public static void FullFillRectTransform(Component comp)
        {
            FullFillRectTransform(comp, Vector2.zero, Vector2.zero);
        }
        public static InputField CreateInputField(string name)
        {
            InputField inputField = new GameObject(name, typeof(Image)).AddComponent<InputField>();
            Text text = CreateText("Text");
            SetParent(text, inputField);
            FullFillRectTransform(text);
            Text placeHolder = CreateText("Placeholder");
            SetParent(placeHolder, inputField);
            FullFillRectTransform(placeHolder);
            inputField.textComponent = text;
            inputField.placeholder = placeHolder;
            return inputField;
        }
        public static Button CreateBtn(string name, string content, Transform parent)
        {
            Button btn = new GameObject(name, typeof(Button), typeof(RectTransform), typeof(Image)).GetComponent<Button>();
            SetParent(btn, parent);
            Image image = btn.GetComponent<Image>();
            //image.sprite = Resources.Load<Sprite>("unity_builtin_extra/UISprite");
            btn.targetGraphic = image;
            Text text = CreateText("Text");
            text.text = content;
            SetParent(text, btn);
            FullFillRectTransform(text, Vector2.zero, Vector2.zero);
            return btn;
        }
        public static Button CreateBtn(string name, string content)
        {
            Button btn = new GameObject(name, typeof(Button), typeof(RectTransform), typeof(Image)).GetComponent<Button>();
            Image image = btn.GetComponent<Image>();
            //image.sprite = Resources.Load<Sprite>("unity_builtin_extra/UISprite");
            btn.targetGraphic = image;
            Text text = CreateText("Text");
            text.text = content;
            SetParent(text, btn);
            FullFillRectTransform(text, Vector2.zero, Vector2.zero);
            return btn;
        }
        public static void SetBtnContent(Button btn, string content)
        {
            btn.transform.Find("Text").GetComponent<Text>().text = content;
        }
        public static Image CreateImage(string name)
        {
            return new GameObject(name, typeof(Image)).GetComponent<Image>();
        }
        public static Text CreateText(string name, string content = "")
        {
            Text text = new GameObject(name, typeof(Text)).GetComponent<Text>();
            text.color = Color.black;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 20;
            text.alignment = TextAnchor.MiddleCenter;
            text.text = content;
            return text;
        }
        public static void SetParent(Component child, Component parent)
        {
            child.transform.SetParent(parent.transform);
            child.transform.localPosition = Vector2.zero;
        }
        public static void CenterAt(Component comp, Vector2 anchor, Vector2 size)
        {
            RectTransform rect = comp.GetComponent<RectTransform>();
            rect.pivot = new Vector2(0.5F, 0.5F);
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.offsetMax = size / 2;
            rect.offsetMin = -rect.offsetMax;
        }
        public static void CenterAt(Component comp, Vector2 anchor, Vector2 size, Vector2 vector)
        {
            RectTransform rect = comp.GetComponent<RectTransform>();
            rect.pivot = new Vector2(0.5F, 0.5F);
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.offsetMin = vector - size / 2;
            rect.offsetMax = vector + size / 2;
        }
        public static void HLineAt(Component comp, float anchor, float height)
        {
            RectTransform rect = comp.GetComponent<RectTransform>();
            rect.pivot = new Vector2(0.5F, 0.5F);
            rect.anchorMin = new Vector2(0, anchor);
            rect.anchorMax = new Vector2(1, anchor);
            rect.offsetMax = new Vector2(0, height / 2);
            rect.offsetMin = -rect.offsetMax;
        }
        public static void VLineAt(Component comp, float anchor, float top, float bottom, float width)
        {
            RectTransform rect = comp.GetComponent<RectTransform>();
            rect.pivot = new Vector2(0.5F, 0.5F);
            rect.anchorMax = new Vector2(anchor, top);
            rect.anchorMin = new Vector2(anchor, bottom);
            rect.offsetMax = new Vector2(width / 2, 0);
            rect.offsetMin = -rect.offsetMax;
        }
        public static void RightTop(Component comp, Vector2 pivot, Vector2 size, Vector2 vector)
        {
            RectTransform rect = comp.GetComponent<RectTransform>();
            rect.pivot = pivot;
            rect.anchorMin = Vector2.one;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = vector + (size * -pivot);
            rect.offsetMax = rect.offsetMin + size;
        }
        public static void RightCenter(Component comp, Vector2 pivot, Vector2 size, Vector2 vector)
        {
            RectTransform rect = comp.GetComponent<RectTransform>();
            rect.pivot = pivot;
            rect.anchorMin = new Vector2(1F, 0.5F);
            rect.anchorMax = rect.anchorMin;
            rect.offsetMin = vector + (size * -pivot);
            rect.offsetMax = rect.offsetMin + size;
        }
        public static void RightBottom(Component comp, Vector2 pivot, Vector2 size, Vector2 vector)
        {
            RectTransform rect = comp.GetComponent<RectTransform>();
            rect.pivot = pivot;
            rect.anchorMin = new Vector2(1, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.offsetMin = vector + (size * -pivot);
            rect.offsetMax = rect.offsetMin + size;
        }
        public static void TopFull(Component comp, float height)
        {
            RectTransform rect = comp.GetComponent<RectTransform>();
            rect.pivot = new Vector2(0.5F, 1F);
            rect.anchorMin = new Vector2(0F, 1F);
            rect.anchorMax = new Vector2(1F, 1F);
            rect.offsetMin = new Vector2(0F, -height);
            rect.offsetMax = Vector2.zero;
        }
        public static void LeftTop(Component comp, Vector2 pivot, Vector2 size, Vector2 vector)
        {
            RectTransform rect = comp.GetComponent<RectTransform>();
            rect.pivot = pivot;
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.offsetMin = vector + (size * -pivot);
            rect.offsetMax = rect.offsetMin + size;
        }
        public static void LeftTop(Component comp, Vector2 pivot, Vector2 size)
        {
            LeftTop(comp, pivot, size, Vector2.zero);
        }
    }
    public static class GOTool
    {
        public static SpriteRenderer CreateSpriteRenderer(string name, Transform parent, bool active = true)
        {
            SpriteRenderer sRenderer = new GameObject(name).AddComponent<SpriteRenderer>();
            sRenderer.gameObject.SetActive(active);
            SetParent(sRenderer, parent);
            return sRenderer;
        }
        public static T ForceGetComponentInChildren<T>(GameObject gameo, string name, bool active = true)
            where T : Component
        {
            Transform transform = gameo.transform.Find(name);
            T t;
            if (transform == null)
            {
                transform = new GameObject(name, typeof(T)).GetComponent<Transform>();
                SetParent(transform, gameo.GetComponent<Transform>());
            }
            t = CompTool.ForceGetComponent<T>(transform);
            t.gameObject.SetActive(active);
            return t;
        }
        public static T ForceGetComponentInChildren<T>(Component comp, string name, bool active = true)
            where T : Component
        {
            return ForceGetComponentInChildren<T>(comp.gameObject, name, active);
        }
        public static void SetParent(Transform child, Transform parent)
        {
            child.SetParent(parent);
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.identity;
        }
        public static void SetParent(Component child, Transform parent)
        {
            SetParent(child.GetComponent<Transform>(), parent);
        }
    }
    public struct Matrix2x2Int
    {
        /// <summary>
        /// m00 m01
        /// m10 m11
        /// </summary>
        private int m00, m01, m10, m11;

        public Matrix2x2Int(int m00, int m01, int m10, int m11)
        {
            this.m00 = m00;
            this.m01 = m01;
            this.m10 = m10;
            this.m11 = m11;
        }
        /// <summary>
        /// 顺时针旋转90度
        ///  0 -1
        ///  1  0
        /// </summary>
        private static Matrix2x2Int roate90Clockwise = new Matrix2x2Int
        {
            m00 = 0,
            m01 = -1,
            m10 = 1,
            m11 = 0
        };
        /// <summary>
        /// 逆时针旋转90度
        ///  0  1
        /// -1  0
        /// </summary>
        private static Matrix2x2Int roate90Anticlockwise = new Matrix2x2Int
        {
            m00 = 0,
            m01 = 1,
            m10 = -1,
            m11 = 0
        };
        /// <summary>
        /// 旋转180度
        ///  -1   0
        ///   0  -1
        /// </summary>
        private static Matrix2x2Int roate180 = new Matrix2x2Int
        {
            m00 = -1,
            m01 = 0,
            m10 = 0,
            m11 = -1
        };
        /// <summary>
        /// 矩阵右乘向量 
        /// vector = vector * matrix
        /// </summary>
        /// <param name="vector">向量</param>
        /// <param name="rhs">矩阵</param>
        /// <returns>转化后的向量</returns>
        private void RightMultipy(ref Vector2Int vector)
        {
            int x = vector.x;
            int y = vector.y;
            vector.x = x * m00 + y * m10;
            vector.y = x * m01 + y * m11;
        }
        /// <summary>
        ///矢量顺时针旋转90度
        /// </summary>
        /// <param name="vector">矢量</param>
        public static void Roate90Clockwise(ref Vector2Int vector)
        {
            roate90Clockwise.RightMultipy(ref vector);
        }
        public static Vector2Int Roate90Clockwise(Vector2Int vector)
        {
            roate90Clockwise.RightMultipy(ref vector);
            return vector;
        }
        /// <summary>
        /// 矢量逆时针旋转90度
        /// </summary>
        /// <param name="vector"></param>
        public static void Roate90Anticlockwise(ref Vector2Int vector)
        {
            roate90Anticlockwise.RightMultipy(ref vector);
        }
        public static Vector2Int Roate90Anticlockwise(Vector2Int vector)
        {
            roate90Anticlockwise.RightMultipy(ref vector);
            return vector;
        }
        /// <summary>
        /// 矢量选择180度
        /// </summary>
        /// <param name="vector"></param>
        public static void Roate180(ref Vector2Int vector)
        {
            vector.x = -vector.x;
            vector.y = -vector.y;
        }
        public static void Roate180(ref Vector2 vector)
        {
            vector.x = -vector.x;
            vector.y = -vector.y;
        }
        public static Vector2Int Roate180(Vector2Int vector)
        {
            vector.x = -vector.x;
            vector.y = -vector.y;
            return vector;
        }
    }
    public struct Pair<T1, T2>
    {
        public T1 one;
        public T2 two;
    }
}
