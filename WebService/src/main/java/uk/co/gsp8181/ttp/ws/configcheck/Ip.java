package uk.co.gsp8181.ttp.ws.configcheck;

import javax.xml.bind.annotation.XmlRootElement;

/**
 * Created by Geoffrey on 21/03/2015.
 */
@XmlRootElement
public class Ip {
    public Ip(String ip)
    {
        this.ip = ip;
    }
    private String ip;

    public String getIp() {
        return ip;
    }

    public void setIp(String ip) {
        this.ip = ip;
    }
}
