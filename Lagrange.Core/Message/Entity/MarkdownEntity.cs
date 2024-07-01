using Lagrange.Core.Internal.Packets.Message.Element;
using Lagrange.Core.Internal.Packets.Message.Element.Implementation;
using Lagrange.Core.Utility.Extension;
using ProtoBuf;

namespace Lagrange.Core.Message.Entity;

[MessageElement(typeof(CommonElem))]
public class MarkdownEntity : IMessageEntity
{
    public MarkdownData Data { get; set; }

    internal MarkdownEntity() => Data = new MarkdownData();

    public MarkdownEntity(MarkdownData data) => Data = data;

    public MarkdownEntity(string data) => Data = new MarkdownData() { Content = data };

    IEnumerable<Elem> IMessageEntity.PackElement() => new Elem[]
    {
        new()
        {
            CommonElem = new CommonElem
            {
                ServiceType = 45,
                PbElem = Data.Serialize().ToArray(),
                BusinessType = 1
            }
        }
    };

    IMessageEntity? IMessageEntity.UnpackElement(Elem elem)
    {
        if (elem is { CommonElem: { ServiceType: 45, BusinessType: 1 } common })
        {
            var markdown = Serializer.Deserialize<MarkdownData>(common.PbElem.AsSpan());
            return new MarkdownEntity(markdown);
        }

        return null;
    }

    public string ToPreviewString() => $"[{nameof(MarkdownEntity)}] {Data.Content}";
}

[ProtoContract]
public class MarkdownData
{
    [ProtoMember(1)]
    public string Content { get; set; } = string.Empty;
}