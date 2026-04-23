using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace The_SimpleNotes_System.Models
{
    public class ApiResponseDto
    { [JsonPropertyName("msg")] 
     public string? Msg { get; set; }
    }
}
