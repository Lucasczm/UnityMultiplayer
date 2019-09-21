using System.Collections;
using System.Collections.Generic;
using Multiplayer;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager instance;
    void Awake()
    {
        if (instance != null) return;
        instance = this;
    }
    #endregion

    public GameObject progressBar, scorePanel;
    Image bar;
    int playerID1, playerID2, score1,score2;
    TMPro.TextMeshProUGUI player1Name, player2Name, player1Score, player2Score;
    void Start()
    {
        bar = progressBar.transform.Find("Bar").GetComponent<Image>();
        player1Score = scorePanel.transform.Find("Player1Score").GetComponent<TMPro.TextMeshProUGUI>();
        player2Score = scorePanel.transform.Find("Player2Score").GetComponent<TMPro.TextMeshProUGUI>();
    }

    public static void UpdateBar(float current, float max)
    {
        if (instance == null) return;
        instance.bar.fillAmount = current / max;
    }

    public static void IncreaseScore(int playerID)
    {
        if(instance == null) return;
        if(ClientManager.instance.myPlayer.ID != playerID)
        {
            instance.score1++;
            instance.player1Score.SetText(instance.score1.ToString());
        }else
        {
            instance.score2++;
            instance.player2Score.SetText(instance.score2.ToString());
        }
    }
}
