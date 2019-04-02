/*
 * 描述：
 * 作者：NONE
 * 创建时间：4/2/2019 2:14:38 PM
 * 版本：v0.7
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldBattle {
    public class EnermyModelUpdate : MonoBehaviour
    {
        public Sprite model1;
        public Sprite model2;
        public Sprite model3;
        public BattleActor parentComponentAI;
        // Start is called before the first frame update
        void Start()
        {
            UpdateModel();

        }

        // Update is called once per frame
        void Update()
        {
        }

        void UpdateModel()
        {
            if (GetComponentInParent<PersonAI>())
                parentComponentAI = GetComponentInParent<PersonAI>();
            else if (GetComponentInParent<EnemyAI>())
                parentComponentAI = GetComponentInParent<EnemyAI>();
            else if (GetComponentInParent<type1AI>())
                parentComponentAI = GetComponentInParent<type1AI>();
            else if (GetComponentInParent<type2AI>())
                parentComponentAI = GetComponentInParent<type2AI>();
            else if (GetComponentInParent<type3AI>())
                parentComponentAI = GetComponentInParent<type3AI>();
            else if (GetComponentInParent<type4AI>())
                parentComponentAI = GetComponentInParent<type4AI>();
            else Debug.Log("无法捕捉AI组件！");

            if (parentComponentAI)
            {
                switch (parentComponentAI.model)
                {
                    case 1:
                        this.GetComponent<SpriteRenderer>().sprite = model1;
                        break;
                    case 2:
                        //野兽太多了，为展示效果，分下流
                        int a = Random.Range(0, 100);
                        if (a < 25)
                            this.GetComponent<SpriteRenderer>().sprite = model1;
                        else if (a > 75) this.GetComponent<SpriteRenderer>().sprite = model3;
                        else
                            this.GetComponent<SpriteRenderer>().sprite = model2;
                        break;
                    case 4:
                        this.GetComponent<SpriteRenderer>().sprite = model3;
                        break;
                    default:
                        this.GetComponent<SpriteRenderer>().sprite = model1;
                        break;
                }
            }
        }
    }
}