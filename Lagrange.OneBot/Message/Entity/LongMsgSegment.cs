using System.Text.Json.Serialization;
using Lagrange.Core.Message;
using Lagrange.Core.Message.Entity;

namespace Lagrange.OneBot.Message.Entity;

[Serializable]
public partial class LongMsgSegment(string id)
{
    public LongMsgSegment() : this("") { }

    [JsonPropertyName("id")] [CQProperty] public string Id { get; set; } = id;
}

[SegmentSubscriber(typeof(LongMsgSegment), "long_msg")]
public partial class LongMsgSegment : SegmentBase
{
    public override void Build(MessageBuilder builder, SegmentBase segment)
    {
        if (segment is LongMsgSegment longMsg) builder.Add(new LongMsgEntity(longMsg.Id));
    }

    public override SegmentBase FromEntity(MessageChain chain, IMessageEntity entity)
    {
        if (entity is not LongMsgEntity longMsg) throw new ArgumentException("Invalid entity type.");

        return new ForwardSegment(longMsg.ResId ?? "");
    }
}