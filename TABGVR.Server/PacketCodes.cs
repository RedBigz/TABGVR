namespace TABGVR.Server;

public enum PacketCodes : byte
{
    /*
     * Interrogate - Forces client to identify their VR Status
     *
     * Server Request: Empty
     * Client Response: Empty
     */
    Interrogate = 196,
    /*
     * Send/receive controller data
     *
     * Client Request:
     *  double hmdPosX,
     *  double hmdPosY,
     *  double hmdPosZ,
     *
     *  double hmdRotX,
     *  double hmdRotY,
     *  double hmdRotZ,
     *
     *  double leftControllerPosX,
     *  double leftControllerPosY,
     *  double leftControllerPosZ,
     *
     *  double leftControllerRotX,
     *  double leftControllerRotY,
     *  double leftControllerRotZ,
     *
     *  double rightControllerPosX,
     *  double rightControllerPosY,
     *  double rightControllerPosZ,
     *
     *  double rightControllerRotX,
     *  double rightControllerRotY,
     *  double rightControllerRotZ,
     * 
     * Server Response: Empty
     *
     * Server Request:
     *  byte playerIndex,
     *
     *  double hmdPosX,
     *  double hmdPosY,
     *  double hmdPosZ,
     *
     *  double hmdRotX,
     *  double hmdRotY,
     *  double hmdRotZ,
     *
     *  double leftControllerPosX,
     *  double leftControllerPosY,
     *  double leftControllerPosZ,
     *
     *  double leftControllerRotX,
     *  double leftControllerRotY,
     *  double leftControllerRotZ,
     *
     *  double rightControllerPosX,
     *  double rightControllerPosY,
     *  double rightControllerPosZ,
     *
     *  double rightControllerRotX,
     *  double rightControllerRotY,
     *  double rightControllerRotZ,
     *  
     * Client Response: Empty
     */
    ControllerMotion = 216,
}