using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public Action<Dictionary<string, string>> OnLanguageUpdate;

    public TMP_FontAsset chineseFontAsset;

    public Dictionary<string, string> CurrentLanguage { get; private set; }

    private Dictionary<string, string> russian = new Dictionary<string, string>()
    {
        ["language"] = "russian",
        ["waiting_screen_text"] = "Ожидаем других игроков",
        ["settings_title"] = "НАСТРОЙКИ",
        ["settings_button_continue"] = "ПРОДОЛЖИТЬ",
        ["bid_description"] = "Ставка:",
        ["multiplier_description"] = "Текущий множитель:",
        ["ingame_button_exit"] = "Выход",
        ["emoji_title"] = "Выберите эмодзи",
        ["emoji_button_continue"] = "Вернуться в игру",
        ["emoji_player_select_title"] = "Кому отправить?",
        ["win_popup_title"] = "ПОБЕДА!",
        ["win_popup_button_exit"] = "В главное меню",
        ["lose_popup_title"] = "ПОРАЖЕНИЕ",
        ["lose_popup_button_exit"] = "В главное меню",
        ["warning_screen_rotate_text"] = "Пожалуйста, переведите экран устройства в горизонтальное положение!",
        ["info_panel_title"] = "Правила игры",
        ["label_pass"] = "Пас",

        ["button_lls_pass"] = "ПАС",
        ["button_lls_call"] = "ПОДНЯТЬ СТАВКУ",

        ["button_mg_pass"] = "ПАС",
        ["button_mg_hint"] = "ПОДСКАЗКА",
        ["button_mg_play"] = "ИГРАТЬ",

        ["info_tab0_0"] = "В свой ход вы должны сыграть любую из доступных комбинаций \n(комбинация может быть побеждена такой же комбинацией, но старше, бомбой или ракетой). ",
        ["info_tab0_1"] = "Комбинации:",
        ["info_tab0_2"] = "Одиночная карта:",
        ["info_tab0_3"] = "ПАРА",
        ["info_tab0_4"] = "ТРОЙКА",
        ["info_tab0_5"] = "ТРОЙКА + Любая одиночная карта",
        ["info_tab0_6"] = "ТРОЙКА + Любая ПАРА",
        ["info_tab0_7"] = "ЧЕТВЕРКА + Любая ПАРА",
        ["info_tab0_8"] = "СТРИТ",
        ["info_tab0_9"] = "ДВОЙНОЙ СТРИТ",
        ["info_tab0_10"] = "ТРОЙНОЙ СТРИТ (САМОЛЕТ)",
        ["info_tab0_11"] = "БОМБА",
        ["info_tab0_12"] = "РАКЕТА",
        ["info_tab0_13"] = "карта, достоинством 3/4/5/6/7/8/9/10/J/Q/K/A/2/Черный джокер/Красный джокер.",
        ["info_tab0_14"] = "не менее пяти карт последовательно, от 3 до туза - например, 8-9-10-J-Q.",
        ["info_tab0_15"] = "пара джокеров. Это самая высокая комбинация, которая превосходит все остальные, включая бомбы.",
        ["info_tab0_16"] = "четыре карты одного достоинства. бомба может победить все, кроме ракеты, а бомба более высокого ранга может победить бомбу более низкого ранга.",
        ["info_tab0_17"] = "две карты с одним значением, любой масти.",
        ["info_tab0_18"] = "три карты с одним значением, любой масти.",
        ["info_tab0_19"] = "например, 8-8-9-9-10-10.",
        ["info_tab0_20"] = "например, 8-8-8-9-9-9.",

        ["info_tab1_0"] = "При входе в комнату выбирается начальная ставка (в валюте). Далее на неё будет влиять только множитель. Также можем назвать начальную ставку, как “Условная единица”.\n \nЗатем производится “Аукцион” на выбор землевладельца. В процессе игроки поднимают начальную ставку от 1-й до 3-х условных единиц (Множитель x1 - x3). Победитель становится Землевладельцем. Итоговая ставка (уже умноженная и рассчитанная для текущего пользователя) и множитель отображаются вверху экрана.",
        ["info_tab1_1"] = "В последствие на ставку влияет выкладывание на стол данных комбинаций:",
        ["info_tab1_2"] = "Бомба (4 одинаковые карты) \nРакета (2 джокера)",
        ["info_tab1_3"] = "При их выкладывании ставка (и, соответственно, множитель) увеличиваются в 2 раза.",
        ["info_tab1_4"] = "Крестьянин (лягушка) выигрывает или проигрывает сумму в полном размере. \nЗемлевладелец (свинья) выигрывает или проигрывает суммы обоих лягушек. \nПри наличии достаточной суммы у обоих крестьян - ставка землевладельца вдвое больше ставки крестьянина.",

        ["tutorial_hint_continue"] = "Нажмите, чтобы продолжить",
        ["tutorial_0"] = "Добро пожаловать в игру Dou Di Zhu! В этой игре ваша главная задача — как можно быстрее избавиться от карт!",
        ["tutorial_1"] = "Это также командная игра. Два Крестьянина (лягушки) играют вместе против Землевладельца (свиньи). В то время как свинья забирает полный выигрыш, лягушки делят его между собой, но также они и рискуют меньше!",
        ["tutorial_2"] = "После того, как вы получили карты, вам нужно оценить их. Если вы считаете, что вам по силам играть за землевладельца, то нажмите «Поднять ставку», а иначе можете спасовать. Голосование идет до 3 очков, и тот, кто сделает большую ставку, становится землевладельцем. Если все игроки спасуют, карты раздаются снова.",
        ["tutorial_3"] = "Земдевладелец также получает 3 карты со стола, чтобы усилить свою колоду, но эти карты видят все игроки!  Затем начинается первый ход.",
        ["tutorial_4"] = "В свой ход вы должны сыграть любую из следующих комбинаций" +
            "\n(комбинация может быть побеждена такой же комбинацией, но старше," +
            "\nбомбой или ракетой)." +
            "\nКомбинации:" +
            "\n-Одиночная карта, достоинством 3/4/5/6/7/8/9/10/J/Q/K/A/2/Черный джокер/Красный джокер" +
            "\n-ПАРА (карты с одним значением, любой масти)" +
            "\n-ТРОЙКА (карты с одним значением, любой масти)" +
            "\n-ТРОЙКА + Любая одиночная карта" +
            "\n-ТРОЙКА + Любая ПАРА" +
            "\n-СТРИТ - не менее пяти карт последовательно, от 3 до туза - например, 8-9-10-J-Q." +
            "\n-ДВОЙНОЙ СТРИТ - например, 8-8-9-9-10-10." +
            "\n-ТРОЙНОЙ СТРИТ- например, 8-8-8-9-9-9." +
            "\n-БОМБА - четыре карты одного достоинства. бомба может победить все, кроме ракеты, а бомба более высокого ранга может победить бомбу более низкого ранга." +
            "\n-РАКЕТА - пара джокеров. Это самая высокая комбинация, которая превосходит все остальные, включая бомбы.",
        ["tutorial_5"] = "Вы также можете пропустить ход в любой момент, если у вас нет подходящей комбинации, или использовать подсказку. \nУдачной игры!"
    };

    private Dictionary<string, string> english = new Dictionary<string, string>()
    {
        ["language"] = "english",
        ["waiting_screen_text"] = "Waiting for other players",
        ["settings_title"] = "SETTINGS",
        ["settings_button_continue"] = "CONTINUE",
        ["bid_description"] = "Bid:",
        ["multiplier_description"] = "Current multiplier:",
        ["ingame_button_exit"] = "Exit",
        ["emoji_title"] = "Choose an emoji",
        ["emoji_button_continue"] = "Return to game",
        ["emoji_player_select_title"] = "Send to whom?",
        ["win_popup_title"] = "VICTORY!",
        ["win_popup_button_exit"] = "To main menu",
        ["lose_popup_title"] = "DEFEAT",
        ["lose_popup_button_exit"] = "To main menu",
        ["warning_screen_rotate_text"] = "Please rotate your device to landscape orientation!",
        ["info_panel_title"] = "Game rules",
        ["label_pass"] = "Pass",

        ["button_lls_pass"] = "PASS",
        ["button_lls_call"] = "RAISE BID",

        ["button_mg_pass"] = "PASS",
        ["button_mg_hint"] = "HINT",
        ["button_mg_play"] = "PLAY",

        ["info_tab0_0"] = "On your turn, you must play any of the available combinations \n(a combination can be beaten by the same combination but higher, a bomb, or a rocket).",
        ["info_tab0_1"] = "Combinations:",
        ["info_tab0_2"] = "Single card:",
        ["info_tab0_3"] = "PAIR",
        ["info_tab0_4"] = "THREE OF A KIND",
        ["info_tab0_5"] = "THREE OF A KIND + Any single card",
        ["info_tab0_6"] = "THREE OF A KIND + Any PAIR",
        ["info_tab0_7"] = "FOUR OF A KIND + Any PAIR",
        ["info_tab0_8"] = "STRAIGHT",
        ["info_tab0_9"] = "DOUBLE STRAIGHT",
        ["info_tab0_10"] = "TRIPLE STRAIGHT (AIRPLANE)",
        ["info_tab0_11"] = "BOMB",
        ["info_tab0_12"] = "ROCKET",
        ["info_tab0_13"] = "card with value 3/4/5/6/7/8/9/10/J/Q/K/A/2/Black Joker/Red Joker.",
        ["info_tab0_14"] = "at least five consecutive cards from 3 to Ace - e.g., 8-9-10-J-Q.",
        ["info_tab0_15"] = "pair of Jokers. This is the highest combination that beats all others, including bombs.",
        ["info_tab0_16"] = "four cards of the same rank. A bomb can beat everything except a rocket, and a higher-ranked bomb can beat a lower-ranked bomb.",
        ["info_tab0_17"] = "two cards of the same value, any suit.",
        ["info_tab0_18"] = "three cards of the same value, any suit.",
        ["info_tab0_19"] = "e.g., 8-8-9-9-10-10.",
        ["info_tab0_20"] = "e.g., 8-8-8-9-9-9.",

        ["info_tab1_0"] = "When entering a room, the initial bid (in currency) is selected. Later, only the multiplier will affect it. We can also call the initial bid a \"Conditional unit\".\n\nThen an \"Auction\" is held to choose the Landlord. During this process, players raise the initial bid from 1 to 3 conditional units (Multiplier x1 - x3). The winner becomes the Landlord. The final bid (already multiplied and calculated for the current user) and the multiplier are displayed at the top of the screen.",
        ["info_tab1_1"] = "Subsequently, the bid is affected by playing the following combinations:",
        ["info_tab1_2"] = "Bomb (4 identical cards) \nRocket (2 Jokers)",
        ["info_tab1_3"] = "When these are played, the bid (and accordingly, the multiplier) doubles.",
        ["info_tab1_4"] = "Peasant (frog) wins or loses the full amount. \nLandlord (pig) wins or loses the amounts of both frogs. \nIf both peasants have sufficient funds, the landlord's bid is twice the peasant's bid.",

        ["tutorial_hint_continue"] = "Tap to continue",
        ["tutorial_0"] = "Welcome to Dou Di Zhu! In this game, your main goal is to get rid of your cards as quickly as possible!",
        ["tutorial_1"] = "This is also a team game. Two Peasants (frogs) play together against the Landlord (pig). While the pig takes the full winnings, the frogs split it between them, but they also risk less!",
        ["tutorial_2"] = "After you receive your cards, you need to evaluate them. If you think you can play as the landlord, press \"Raise the bid\", otherwise you can pass. The bidding goes up to 3 points, and the one who makes the highest bid becomes the landlord. If all players pass, the cards are dealt again.",
        ["tutorial_3"] = "The Landlord also gets 3 cards from the table to strengthen their deck, but all players see these cards! Then the first turn begins.",
        ["tutorial_4"] = "On your turn, you must play any of the following combinations" +
            "\n(a combination can be beaten by the same combination but higher," +
            "\na bomb, or a rocket)." +
            "\nCombinations:" +
            "\n-Single card with value 3/4/5/6/7/8/9/10/J/Q/K/A/2/Black Joker/Red Joker" +
            "\n-PAIR (two cards of the same value, any suit)" +
            "\n-THREE OF A KIND (three cards of the same value, any suit)" +
            "\n-THREE OF A KIND + Any single card" +
            "\n-THREE OF A KIND + Any PAIR" +
            "\n-STRAIGHT - at least five consecutive cards from 3 to Ace - e.g., 8-9-10-J-Q." +
            "\n-DOUBLE STRAIGHT - e.g., 8-8-9-9-10-10." +
            "\n-TRIPLE STRAIGHT - e.g., 8-8-8-9-9-9." +
            "\n-BOMB - four cards of the same rank. A bomb can beat everything except a rocket, and a higher-ranked bomb can beat a lower-ranked bomb." +
            "\n-ROCKET - pair of Jokers. This is the highest combination that beats all others, including bombs.",
        ["tutorial_5"] = "You can also skip your turn at any time if you don't have a suitable combination, or use a hint. \nGood luck!"
    };

    private Dictionary<string, string> chinese = new Dictionary<string, string>()
    {
        ["language"] = "chinese",
        ["waiting_screen_text"] = "等待其他玩家",
        ["settings_title"] = "设置",
        ["settings_button_continue"] = "继续",
        ["bid_description"] = "底分:",
        ["multiplier_description"] = "当前倍数:",
        ["ingame_button_exit"] = "退出",
        ["emoji_title"] = "选择表情",
        ["emoji_button_continue"] = "返回游戏",
        ["emoji_player_select_title"] = "发送给谁?",
        ["win_popup_title"] = "胜利!",
        ["win_popup_button_exit"] = "返回主菜单",
        ["lose_popup_title"] = "失败",
        ["lose_popup_button_exit"] = "返回主菜单",
        ["warning_screen_rotate_text"] = "请将设备横屏放置!",
        ["info_panel_title"] = "游戏规则",
        ["label_pass"] = "不叫",

        ["button_lls_pass"] = "不叫",
        ["button_lls_call"] = "叫地主",

        ["button_mg_pass"] = "不出",
        ["button_mg_hint"] = "提示",
        ["button_mg_play"] = "出牌",

        ["info_tab0_0"] = "在你的回合，你必须出任意可用的牌型\n(同样的牌型但更大的可以压制，炸弹或火箭也能压制)。",
        ["info_tab0_1"] = "牌型:",
        ["info_tab0_2"] = "单张:",
        ["info_tab0_3"] = "对子",
        ["info_tab0_4"] = "三张",
        ["info_tab0_5"] = "三带一",
        ["info_tab0_6"] = "三带二",
        ["info_tab0_7"] = "四带二",
        ["info_tab0_8"] = "顺子",
        ["info_tab0_9"] = "双顺",
        ["info_tab0_10"] = "三顺(飞机)",
        ["info_tab0_11"] = "炸弹",
        ["info_tab0_12"] = "火箭",
        ["info_tab0_13"] = "单张: 3/4/5/6/7/8/9/10/J/Q/K/A/2/小王/大王。",
        ["info_tab0_14"] = "至少五张连续的单牌，从3到A，例如8-9-10-J-Q。",
        ["info_tab0_15"] = "双王(火箭)。这是最大的牌型，可以压制任何牌包括炸弹。",
        ["info_tab0_16"] = "四张相同的牌。炸弹可以压制除火箭外的所有牌型，更大的炸弹可以压制小的炸弹。",
        ["info_tab0_17"] = "两张相同点数的牌，花色不限。",
        ["info_tab0_18"] = "三张相同点数的牌，花色不限。",
        ["info_tab0_19"] = "例如: 8-8-9-9-10-10。",
        ["info_tab0_20"] = "例如: 8-8-8-9-9-9。",

        ["info_tab1_0"] = "进入房间时选择初始底分(游戏币)。之后只有倍数会影响它。初始底分也可以称为\"基准分\"。\n\n然后进行\"叫地主\"环节。玩家可以将基准分提高到1-3倍(倍数x1-x3)。叫分最高的玩家成为地主。最终底分(已乘以当前倍数)和倍数会显示在屏幕上方。",
        ["info_tab1_1"] = "以下牌型会影响倍数:",
        ["info_tab1_2"] = "炸弹(4张相同的牌)\n火箭(双王)",
        ["info_tab1_3"] = "出这些牌型时，底分(和倍数)会翻倍。",
        ["info_tab1_4"] = "农民(青蛙)赢或输全额。\n地主(猪)赢或输两个农民的总额。\n如果两个农民都有足够金额，地主的底分是农民的两倍。",

        ["tutorial_hint_continue"] = "点击继续",
        ["tutorial_0"] = "欢迎来到斗地主游戏! 你的主要目标是尽快出完手中的牌!",
        ["tutorial_1"] = "这是一个团队游戏。两个农民(青蛙)合作对抗地主(猪)。地主赢得或输掉全部筹码，而农民分摊输赢，所以风险更小!",
        ["tutorial_2"] = "拿到牌后需要评估牌力。如果你觉得可以做地主，就点击\"叫地主\"，否则可以\"不叫\"。叫分最高(最多3分)的玩家成为地主。如果都不叫，则重新发牌。",
        ["tutorial_3"] = "地主会获得3张底牌来增强手牌，这些底牌所有玩家都可见! 然后开始第一轮出牌。",
        ["tutorial_4"] = "在你的回合，你必须出以下任意牌型" +
            "\n(同样的牌型但更大的可以压制，" +
            "\n炸弹或火箭也能压制)。" +
            "\n牌型:" +
            "\n-单张: 3/4/5/6/7/8/9/10/J/Q/K/A/2/小王/大王" +
            "\n-对子(两张同点数的牌，花色不限)" +
            "\n-三张(三张同点数的牌，花色不限)" +
            "\n-三带一" +
            "\n-三带二" +
            "\n-顺子 - 至少五张连续单牌，从3到A，例如8-9-10-J-Q" +
            "\n-双顺 - 例如8-8-9-9-10-10" +
            "\n-三顺(飞机) - 例如8-8-8-9-9-9" +
            "\n-炸弹 - 四张相同的牌。炸弹可以压制除火箭外的所有牌型，更大的炸弹可以压制小的炸弹" +
            "\n-火箭 - 双王。这是最大的牌型，可以压制任何牌包括炸弹。",
        ["tutorial_5"] = "你也可以随时选择不出牌，或者使用提示功能。\n祝你好运!"
    };

    void Awake()
    {
        SetLanguageToEnglish();
    }

    public void SetLanguageByValue(Languages lang)
    {
        switch (lang)
        {
            case Languages.english:
                SetLanguageToEnglish();
                break;
            case Languages.russian:
                SetLanguageToRussian();
                break;
            case Languages.chinese:
                SetLanguageToChinese();
                break;
        }
        OnLanguageUpdate?.Invoke(CurrentLanguage);
    }

    public void SetLanguageToEnglish()
    {
        CurrentLanguage = english;
        OnLanguageUpdate?.Invoke(CurrentLanguage);
    }

    public void SetLanguageToRussian()
    {
        CurrentLanguage = russian;
        OnLanguageUpdate?.Invoke(CurrentLanguage);
    }

    public void SetLanguageToChinese()
    {
        CurrentLanguage = chinese;
        OnLanguageUpdate?.Invoke(CurrentLanguage);
    }
}
