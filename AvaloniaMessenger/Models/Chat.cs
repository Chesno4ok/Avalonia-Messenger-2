﻿using DynamicData.Cache.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AvaloniaMessenger.Models;

[JsonObject(MemberSerialization.OptIn)]
public class Chat
{
    [JsonProperty]
    public int Id { get; set; }

    [JsonProperty]
    public string ChatName { get; set; } = null!;
    public string? LastMessage { get; set; }

}
