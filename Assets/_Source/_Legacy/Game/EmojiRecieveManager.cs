using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Web.New.Messages.Payloads.ClientServer;
using Web.New.Messages.Payloads.ServerClient;

public class EmojiManager : MonoBehaviour
{
    public PlayerStatsController mainPlayer;
    public PlayerStatsController player2;
    public PlayerStatsController player3;

    public ProjectileTarget target1;
    public ProjectileTarget target2;
    public ProjectileTarget target3;
    
    public GameObject[] projectilePrefabs;
    public int selectedEmojiId = 0;

    private WebGameManager webGameManager;

    void Start()
    {
        webGameManager = FindObjectOfType<WebGameManager>();
        if(webGameManager)
            webGameManager.eventRouter.OnEmojiReceived += HandleEmoji;
    }

    private void HandleEmoji(ServerEmojiPayload message)
    {
        //try
        //{
            SendProjectile(projectilePrefabs[message.emoji_id], message);
        //}
        //catch
        //{

        //}
    }

    public void SendToPlayerBySide(bool left)
    {
        if(webGameManager.isMultiplayer)
        {
            long recieverId = left ? player2.playerId : player3.playerId;
            webGameManager.connectionManager.SendEmoji(recieverId, selectedEmojiId);      
        }
        else
        {
            long recieverId = left ? player2.playerId : player3.playerId;
            var ep = new ServerEmojiPayload();
            ep.emoji_id = selectedEmojiId;
            ep.from_user_id = mainPlayer.playerId;
            ep.to_user_id = recieverId;
            HandleEmoji(ep); // for test in Editor
        }
    }

    private void SendProjectile(GameObject projectile, ServerEmojiPayload message)
    {
        ProjectileTarget startPosition = new ProjectileTarget();
        if(mainPlayer.playerId == message.from_user_id)
            startPosition = target1;
        else if(player2.playerId == message.from_user_id)
            startPosition = target2;
        else if(player3.playerId == message.from_user_id)
            startPosition = target3;
        
        ProjectileTarget endPosition = new ProjectileTarget();
        if(mainPlayer.playerId == message.to_user_id)
            endPosition = target1;
        else if(player2.playerId == message.to_user_id)
            endPosition = target2;
        else if(player3.playerId == message.to_user_id)
            endPosition = target3;


        var obj = Instantiate(projectile,transform.parent);
        Debug.Log(obj);
        obj.GetComponent<ProjectileMover>().Move(startPosition, endPosition);
    }
}
