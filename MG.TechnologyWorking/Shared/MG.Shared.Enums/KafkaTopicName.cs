using System.ComponentModel;

namespace MG.Shared.Enums
{
    public enum KafkaTopicName
    {
        [Description("general")]
        None = 0,
        [Description("log")]
        Log = 1,
    }
}