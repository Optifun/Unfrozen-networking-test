using Chat.API.DataAccess;
using Chat.API.DTO;
using Mapster;

namespace Chat.API.Mapping
{
    public class MessageConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config
                .ForType<Message, MessageDTO>()
                .Map(dest => dest,
                    src => src.User)
                .Map(dest => dest.Username, src => src.User.Name);
        }
    }
}