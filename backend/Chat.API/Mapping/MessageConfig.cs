using Chat.API.DataAccess;
using Chat.API.DTO;
using Mapster;

namespace Chat.API.Mapping
{
    public class MessageConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<Message, MessageDTO>()
                .Map(dest => new Message()
                    {
                        Text = dest.Text,
                        User = new User()
                        {
                            Name = dest.Username,
                            Color = dest.Color
                        }
                    },
                    src => new MessageDTO(src.User.Name, src.Text, src.User.Color));
        }
    }
}