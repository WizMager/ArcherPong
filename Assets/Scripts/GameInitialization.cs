using System.Collections.Generic;
using Controllers;
using Model;
using Photon.Pun;
using UnityEngine;
using Views;

public class GameInitialization
{
        public GameInitialization(Controllers.Controllers controllers, Data.Data data)
        {
            var bot = Object.Instantiate(data.GetBotData.botPrefab);
            var arrow = Object.Instantiate(data.GetArrowData.soloArrowPrefab);
            var botController = new BotController(bot.transform, arrow.transform, data.GetBotData);
        }
}