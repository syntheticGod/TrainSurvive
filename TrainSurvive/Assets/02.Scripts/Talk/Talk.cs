/*
 * 描述：
 * 作者：李巡
 * 创建时间：2019/1/21 15:34:52
 * 版本：v0.7
 */
using System.Collections.Generic;
using Story.MyTools;
using Story.Faker;
using WorldMap.Model;

namespace Story.Communication{
    public class Talk{

        public static Talk instance = new Talk();

        /// <summary>
        /// 谈谈自己的map，限制最多可以说两条属性
        /// 体力，力量，敏捷，技巧，智力
        /// </summary>
        private Dictionary<int,int[]> selfMap;
        
        private Talk(){
            selfMap = new Dictionary<int, int[]>();
        }

        public enum TalkOpt{
            self,
            recent,
            recruit,
            world,
        }

        public List<string> ask(ref NpcData npc, TalkOpt opt){
            
            List<string> result = new List<string>();

            switch(opt){
                case TalkOpt.self:
                    result.Add(canTalk(npc.ID) ? getSelfInfo(ref npc) : noResultWords(opt) );
                    break;
                case TalkOpt.recent:
                    result.Add(existRandomEvent() ? getRecentEventInfo() : noResultWords(opt) );
                    break;
                case TalkOpt.recruit:
                    result.Add(canRecruit() ? getRecruitWords() : noResultWords(opt) );
                    break;
                case TalkOpt.world:
                    result.Add(getWorldWords());
                    break;
                default:
                    break;
            }
            return result;

        }



        /// <summary>
        /// 相关对话的判断和返回方法，包括
        /// 能否回答，不能的语料库，可以的语料库
        /// </summary>
        /// 

        //  todo 待扩充语料库
        private string noResultWords(TalkOpt opt){
            string result = "";
            switch(opt){
                case TalkOpt.self:
                    result += "无可奉告";
                    break;
                case TalkOpt.recent:
                    result += "风平浪静";
                    break;
                case TalkOpt.recruit:
                    result += "我，你高攀不起";
                    break;
                default:
                    break;
            }
            return result;
        }

        private bool canTalk(int id){
            if(!selfMap.ContainsKey(id)) return true;
            int[] talked = selfMap[id];
            int count = 0;
            for (int i = 0; i < talked.Length; i++){
                if (talked[i] == 1) count++;
            }
            //todo fakevalue
            return count < FakeValue.charm && FakeValue.charm <= 5;//防止修改器修改
        }


        /// <summary>
        /// 暴露自己的属性
        /// todo 待补充语料库
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        private string getSelfInfo(ref NpcData npc){
            int personId = npc.ID;
            if(!selfMap.ContainsKey(personId)){
                int[] initArr = {0,0,0,0,0}; 
                selfMap.Add(personId,initArr);
            }
            int[] stateArr = selfMap[personId];
            int rdSelf = Tools.random(5);
            while(stateArr[rdSelf] == 1){
                rdSelf = Tools.random(5);
            }
            stateArr[rdSelf] = 1;

            return getSelfInfoWords(rdSelf,ref npc);
        }

        //todo 完善语料库，更改叙述方式
        private string getSelfInfoWords(int rdSelf, ref NpcData npc)
        {
            string result = "";
            switch(rdSelf){
                case 0:
                    result += "我的体力是" + npc.PersonInfo.vitality;
                    break;
                case 1:
                    result += "我的力量是" + npc.PersonInfo.strength;
                    break;
                case 2:
                    result += "我的敏捷是" + npc.PersonInfo.agile;
                    break;
                case 3:
                    result += "我的技巧是" + npc.PersonInfo.technique;
                    break;
                case 4:
                    result += "我的智力是" + npc.PersonInfo.intelligence;
                    break;
                default:
                    break;
            }
            return result;
        }


        private bool existRandomEvent(){
            return RandomEventpool.instance.existEvent();
        }

        private string getRecentEventInfo(){
            return "最近zju似乎发生了一件大事";
        }

        private bool canRecruit(){
            return false;
        }

        private string getRecruitWords(){
            return "我加入";
        }

        private string getWorldWords(){
            return "这个愚蠢的世界";
        }
    }
}