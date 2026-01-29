using PureGym.SharedKernel.Events;

namespace PureGym.Mock.RevolvingDoor;

public class DoorState
{
    public GymAccessGrantedEvent? CurrentUser { get; private set; }

    public void SetUser(GymAccessGrantedEvent user)
    {
        CurrentUser = user;
    }

    public void Clear()
    {
        CurrentUser = null;
    }
}
