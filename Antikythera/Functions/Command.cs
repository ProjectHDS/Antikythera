﻿using Antikythera.Utils;

using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Message;

using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Antikythera.Functions;

public class Commands
{
    [Command(CommandType.Common, "/help")]
    private MessageBuilder? OnCommandHelp()
        => new MessageBuilder()
            .Text("[Antikythera Help]\n")
            .Text("/ping\n Pong!\n\n")
            .Text("/help\n 打印本帮助消息。\n\n")
            .Text("/status\n 显示 Bot 状态。\n\n");

    [Command(CommandType.Status, "/status")]
    private MessageBuilder? OnCommandStatus(BotStatus stat)
        => new MessageBuilder()
            // Core descriptions
            .Text($"[Antikythera]\n")
            .Text($"[分支名:{BuildStamp.Branch}]\n")
            .Text($"[版本号:{BuildStamp.Version}]\n")
            .Text($"[提交哈希:{BuildStamp.CommitHash[..12]}]\n")
            .Text($"[构建时间:{BuildStamp.BuildTime}]\n\n")

            // System status
            .Text($"启动至今已处理 {stat.GroupMessageReceived} 条消息。\n")
            .Text($"回收器内存: {GC.GetTotalAllocatedBytes().Bytes2MiB(2)} MiB " +
                  $"({Math.Round((double)GC.GetTotalAllocatedBytes() / GC.GetTotalMemory(false) * 100, 2)}%)\n")
            .Text($"总内存: {Process.GetCurrentProcess().WorkingSet64.Bytes2MiB(2)} MiB\n\n")

            // Copyrights
            .Text("Powered by ProjectHDS, Konata.Core and Kagami Project.");


    [Command(CommandType.Common, "/ping")]
    private MessageBuilder? OnCommandPing() => new MessageBuilder().Text("Pong!");

    [Command(CommandType.Full, "/jrrp")]
    private MessageBuilder? OnCommandJrrp(Bot bot, GroupMessageEvent evt)
    {
        var date = new DateTimeOffset(DateTime.Now).LocalDateTime;
        var salt = "\\u5496\\u55b1";
        var year = date.Year.ToString();
        var month = date.Month.ToString();
        var day = date.Day.ToString();
        year += salt;
        day += salt;
        var bytes = Encoding.UTF8.GetBytes($"{month}+{year}+{day}+{Regex.Unescape(salt)}");
        int seed = 1;
        foreach (var b in bytes)
        {
            seed += (int)b;
        }

        seed += (int)evt.MemberUin;
        var random = new Random(seed);
        var rp = Math.Round(new GaussianRng(seed).Next(), 2) * 100;
        return new MessageBuilder().At(evt.MemberUin).Text($"的今日人品为：{rp}");
    }

    [Command(CommandType.Full, "/zrrp")]
    private MessageBuilder? OnCommandZrrp(Bot bot, GroupMessageEvent evt)
    {
        var date = new DateTimeOffset(DateTime.Now).LocalDateTime;
        var dateP = date.AddDays(-1);
        var salt = "\\u5496\\u55b1";
        var year = dateP.Year.ToString();
        var month = dateP.Month.ToString();
        var day = dateP.Day.ToString();

        year += salt;
        day += salt;
        var bytes = Encoding.UTF8.GetBytes($"{month}+{year}+{day}+{Regex.Unescape(salt)}");
        int seed = 1;
        foreach (var b in bytes)
        {
            seed += (int)b;
        }

        seed += (int)evt.MemberUin;
        var rp = Math.Round(new GaussianRng(seed).Next(), 2) * 100;
        return new MessageBuilder().At(evt.MemberUin).Text($"的昨日人品为：{rp}");
    }
}
