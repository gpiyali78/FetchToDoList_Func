using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FetchToDoListFunc.Model
{
    public class TaskList
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int64)]
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime TaskStartDate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime TaskEndDate { get; set; }
        public string TaskStatus { get; set; }
        public int TotalEffortRequired { get; set; }
    }
}
