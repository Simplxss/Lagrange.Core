using System.Numerics;
using Lagrange.Core.Internal.Event.System;
using Lagrange.Core.Internal.Packets.Message.Component.Extra;
using Lagrange.Core.Internal.Packets.Message.Element;
using Lagrange.Core.Internal.Packets.Message.Element.Implementation;
using Lagrange.Core.Internal.Packets.Message.Element.Implementation.Extra;
using Lagrange.Core.Internal.Packets.Service.Oidb.Common;
using Lagrange.Core.Utility.Extension;
using ProtoBuf;

namespace Lagrange.Core.Message.Entity;

[MessageElement(typeof(CommonElem))]
public class CommonEntity : IMessageEntity
{
    public CommonEntity() { }

    IEnumerable<Elem> IMessageEntity.PackElement() => throw new NotImplementedException();

    IMessageEntity? IMessageEntity.UnpackElement(Elem elems)
    {
        if (elems.CommonElem is { ServiceType: 48 } common)
        {
            var msgInfo = Serializer.Deserialize<MsgInfo>(common.PbElem.AsSpan());
            foreach (var msg in msgInfo.MsgInfoBody)
            {
                var index = msg.Index;
                var uploadTime = index.UploadTime;
                var fileInfo = index.Info;
                var fileType = fileInfo.Type.Type;

                switch (fileType)
                {
                    case 1:
                        {
                            var picture = msg.Picture;
                            var urlPath = picture.UrlPath;
                            var domain = picture.Domain;

                            var subType = msgInfo.ExtBizInfo.Pic.BizType;
                            return new ImageEntity
                            {
                                PictureSize = new Vector2(fileInfo.Width, fileInfo.Height),
                                FilePath = index.FileUuid,
                                ImageSize = fileInfo.FileSize,
                                ImageUrl =  $"https://{domain}{urlPath}",
                                MsgInfo = msgInfo,
                                Summary = msgInfo.ExtBizInfo.Pic.TextSummary,
                                SubType = (int)subType
                            };
                        }
                    default:
                        throw new NotImplementedException();
                }
            }

        }
        // return new ImageEntity
        // {
        //     PictureSize = new Vector2(nt_image.Width, face.Height),
        //     FilePath = face.FilePath,
        //     ImageSize = face.Size,
        //     ImageUrl = $"{LegacyBaseUrl}{face.OrigUrl}",
        //     Summary = face.PbReserve?.Summary,
        //     SubType = face.PbReserve?.SubType ?? GetImageTypeFromFaceOldData(face)
        // };

        return null;
    }

    public string ToPreviewString() => throw new NotImplementedException();
}
