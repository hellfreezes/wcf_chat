using System.ServiceModel;

namespace wcf_chat
{
    class ServerUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public OperationContext Operation { get; set; }
    }
}
