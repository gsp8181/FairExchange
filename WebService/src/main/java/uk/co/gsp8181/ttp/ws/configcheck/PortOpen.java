package uk.co.gsp8181.ttp.ws.configcheck;

/**
 * Created by Geoffrey on 21/03/2015.
 */
public class PortOpen {
    private String ip;
    private int port;
    private boolean isOpen;

    public PortOpen(String ip, int port, boolean isOpen)
    {
        this.ip = ip;
        this.port = port;
        this.isOpen = isOpen;
    }

    public String getIp() {
        return ip;
    }

    public void setIp(String ip) {
        this.ip = ip;
    }

    public int getPort() {
        return port;
    }

    public void setPort(int port) {
        this.port = port;
    }

    public boolean isOpen() {
        return isOpen;
    }

    public void setOpen(boolean isOpen) {
        this.isOpen = isOpen;
    }
}
