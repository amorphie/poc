using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace amorphie.poc.signalr_server.Model;

    public record HubMessage(ChannelNameEnum ChannelNameEnum, ToEnum ToEnum, string? To,string Message);
