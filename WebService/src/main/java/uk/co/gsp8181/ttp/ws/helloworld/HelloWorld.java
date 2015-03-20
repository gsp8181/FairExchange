package uk.co.gsp8181.ttp.ws.helloworld;

import javax.xml.bind.annotation.XmlRootElement;

@XmlRootElement
public class HelloWorld {
    private String val;

    public String getVal() {
        return val;
    }

    public void setVal(String val) {
        this.val = val;
    }

}