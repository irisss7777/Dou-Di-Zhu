using _Source.Contracts.DTO.Card;
using _Source.Contracts.DTO.GameStage;
using _Source.Contracts.DTO.Lobby;
using _Source.Contracts.DTO.Player;
using _Source.Contracts.DTO.Web;
using _Source.Contracts.DTO.Web.SendDTO;
using _Source.Contracts.DTO.Web.WebsocketConnectionDTO;
using MessagePipe;
using Zenject;

namespace _Source.Infrastructure.Installers
{
    public class DTOInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var options = Container.BindMessagePipe();

            WebServiceDTO(options);
            InputDTO(options);
            GameStateDTO(options);
            DomainDTO(options);
            ViewDTO(options);
        }

        private void GameStateDTO(MessagePipeOptions options)
        {
            Container.BindMessageBroker<LobbyInfoStateDTO>(options);
            Container.BindMessageBroker<GameActiveStateDTO>(options);
        }

        private void InputDTO(MessagePipeOptions options)
        {
            Container.BindMessageBroker<SelectInputCardDTO>(options);
            Container.BindMessageBroker<InputConnectionDTO>(options);
            Container.BindMessageBroker<UseCardInputDTO>(options);
        }

        private void WebServiceDTO(MessagePipeOptions options)
        {
            Container.BindMessageBroker<RawMessageReceiveDTO>(options);
            
            //Connection
            Container.BindMessageBroker<ConnectionDTO>(options);
            Container.BindMessageBroker<ErrorDTO>(options);
            
            //Send 
            Container.BindMessageBroker<PlayerConnectSendDTO>(options);
            Container.BindMessageBroker<PlayerPassDTO>(options);
            Container.BindMessageBroker<CanUseCardDTO>(options);
            Container.BindMessageBroker<UseCardDTO>(options);
            
            //Receive 
            Container.BindMessageBroker<PlayerConnectedDTO>(options);
            Container.BindMessageBroker<SidePlayerConnectedDTO>(options);
            Container.BindMessageBroker<PlayerLeaveDTO>(options);
            Container.BindMessageBroker<AllPlayerInfoDTO>(options);
            Container.BindMessageBroker<AddCardDTO>(options);
            Container.BindMessageBroker<SetMoveStateDTO>(options);
            Container.BindMessageBroker<CanUseCardReceiverDTO>(options);
            Container.BindMessageBroker<UseCardReceiverDTO>(options);
            Container.BindMessageBroker<UseCardOtherDTO>(options);
            Container.BindMessageBroker<PlayerPassedDTO>(options);
        }

        private void DomainDTO(MessagePipeOptions options)
        { 
            Container.BindMessageBroker<AddPlayerToLobbyDTO>(options);
            Container.BindMessageBroker<CurrentPlayerAddedDTO>(options);
            Container.BindMessageBroker<SelectCardDTO>(options);
            Container.BindMessageBroker<DeselectCardDTO>(options);
            Container.BindMessageBroker<CardMoveDTO>(options);
            Container.BindMessageBroker<MoveUsedCardsDTO>(options);
            Container.BindMessageBroker<AddCardOtherDTO>(options);
        }

        private void ViewDTO(MessagePipeOptions options)
        {
            Container.BindMessageBroker<SelectViewCardDTO>(options);
            Container.BindMessageBroker<PlayerSetCardCountDTO>(options);
            Container.BindMessageBroker<CardDestroyViewDTO>(options);
            Container.BindMessageBroker<PlayerAddCardViewDTO>(options);
        }
    }
}