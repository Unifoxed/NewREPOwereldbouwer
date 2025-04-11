using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.DTO_Model
{
    [Serializable]
    public class PostWorldCreateRequestDto
    {
        public int maxHeight;
        public int maxLength;
        public string name;
        public string ownerUserId;
        public string id;
    }



}