package uk.co.gsp8181.ttp.sec;

import java.io.Serializable;

public class TestData implements Serializable {

    private final String publicKeyBase64;
    private final String email;
    private final String sigBase64;
    private final String privateKeyBase64;

    public TestData(String publicKeyBase64, String email,
                    String sigBase64, String privateKeyBase64) {
        this.publicKeyBase64 = publicKeyBase64;
        this.email = email;
        this.sigBase64 = sigBase64;
        this.privateKeyBase64 = privateKeyBase64;
    }

    public String getPublicKeyBase64() {
        return publicKeyBase64;
    }

    public String getEmail() {
        return email;
    }

    public String getSigBase64() {
        return sigBase64;
    }

    public String getPrivateKeyBase64() {
        return privateKeyBase64;
    }

    public void print() {
        System.out.println(publicKeyBase64);
        System.out.println(email);
        System.out.println(sigBase64);
        System.out.println(privateKeyBase64);
    }
}