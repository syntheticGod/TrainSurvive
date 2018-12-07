/*
 * 描述：序列化接口
 * 作者：项叶盛
 * 创建时间：2018/11/22 1:41:57
 * 版本：v0.1
 */
namespace WorldMap
{
    public interface Serialize
    {
        Serializable Serialize();
        void Deserialize(Serializable serializable);
    }
    public interface Serializable
    {
    }
}