package uk.co.gsp8181.ttp.sec;

import org.bouncycastle.openssl.PEMWriter;

import java.io.IOException;
import java.io.StringWriter;
import java.security.InvalidKeyException;
import java.security.Key;
import java.security.KeyFactory;
import java.security.KeyPair;
import java.security.KeyPairGenerator;
import java.security.NoSuchAlgorithmException;
import java.security.NoSuchProviderException;
import java.security.PrivateKey;
import java.security.PublicKey;
import java.security.SecureRandom;
import java.security.Signature;
import java.security.SignatureException;
import java.security.spec.InvalidKeySpecException;
import java.security.spec.PKCS8EncodedKeySpec;
import java.security.spec.X509EncodedKeySpec;
import java.time.Duration;
import java.time.Instant;
import java.util.Base64;
import java.util.Base64.Decoder;
import java.util.Base64.Encoder;


public class CertificateTools {
    /**
     * Returns an object containing an example private/public key pair and signed data.
     * Should only be used for testing
     * @param dataToSign The data to sign, usually the username
     * @return The TestData object with the signed data and the keypair
     */
    public static TestData getTestData(String dataToSign) throws NoSuchAlgorithmException, NoSuchProviderException, InvalidKeyException, SignatureException
    {
        KeyPairGenerator keyGen = KeyPairGenerator.getInstance("RSA");
        SecureRandom random = SecureRandom.getInstance("SHA1PRNG", "SUN");
        keyGen.initialize(2048, random);
        KeyPair pair = keyGen.generateKeyPair();

        Signature dsa = Signature.getInstance("SHA1withRSA");

        PrivateKey priv = pair.getPrivate();
        dsa.initSign(priv);
        dsa.update(dataToSign.getBytes());
        byte[] sig = dsa.sign();

        PublicKey pub = pair.getPublic();

        TestData out = new TestData(encodeRSA(pub), dataToSign, encodeBase64(sig), encodeRSA(priv));


        return out;
    }

    public static String getPemForKey(PublicKey publicKey)
    {
        StringWriter stringWriter = new StringWriter();
        PEMWriter pemWriter = new PEMWriter(stringWriter);
        try {
            pemWriter.writeObject( publicKey);
        pemWriter.close();
        } catch (IOException e) { //THIS WILL NOT THROW
            e.printStackTrace();
        }
        return stringWriter.toString();
    }

    /*
     * mvn exec:java -Dexec.mainClass="com.team2.security.CertificateTools" -Dexec.args="priv pub data" TODO:@END:REMOVE
     */
    public static void main(String[] args) throws Exception
    {
        PrivateKey priv = decodeRSAPriv(args[0]);
        PublicKey pub = decodeDSAPub(args[1]);
        KeyPair pubpriv = new KeyPair(pub,priv);

        System.out.println(signData(args[2],pubpriv));
    }

    public static String signData(String dataToSign, KeyPair pair) throws InvalidKeyException, SignatureException, NoSuchAlgorithmException
    {
        return signData(dataToSign, pair.getPrivate());
    }

    public static String signData(String dataToSign, PrivateKey priv) throws InvalidKeyException, SignatureException, NoSuchAlgorithmException
    {
        Signature dsa = Signature.getInstance("SHA1withRSA");

        dsa.initSign(priv);
        dsa.update(dataToSign.getBytes());
        byte[] sig = dsa.sign();

        return encodeBase64(sig);
    }

    private static String base64urlencode(String base64)
    {
        return base64.replace('/', '.').replace('=','\\');
    }

    private static String base64urldecode(String base64url)
    {
        return base64url.replace(' ', '+').replace('.', '/').replace('\\','=');

    }


    /**
     * Encodes a Key object with Base64
     * @param key The PublicKey or PrivateKey to encode
     * @return The base64 encoded key string
     */
    public static String encodeRSA(Key key)
    {
        byte[] array = key.getEncoded();
        return encodeBase64(array);
    }

    public static PrivateKey decodeRSAPriv(String key) throws NoSuchAlgorithmException, InvalidKeySpecException{
        byte[] byteKey = decodeBase64(key);
        PKCS8EncodedKeySpec pkcs8Private = new PKCS8EncodedKeySpec(byteKey);
        KeyFactory kf = KeyFactory.getInstance("RSA");

        return kf.generatePrivate(pkcs8Private);
    }

    /*public static boolean verifyTimestamp(PublicKey key, Instant timestamp, String signedStamp) throws InvalidKeyException, SignatureException, NoSuchAlgorithmException
    {
        Duration between = Duration.between(timestamp,Instant.now());
        long duration = between.toMinutes();
        duration = duration < 0 ? -duration : duration;
        if (duration > 5)
            return false;

        String signedData = String.valueOf(timestamp.getEpochSecond());

        if(verify(key,signedData, base64urldecode(signedStamp)))
            return true;
        return false;
    }

    public static boolean verifyTimestamp(PublicKey key, long timestampSeconds, String signedStamp) throws InvalidKeyException, SignatureException, NoSuchAlgorithmException
    {
        Instant timestamp = Instant.ofEpochSecond(timestampSeconds, 0);
        return verifyTimestamp(key,timestamp,signedStamp);
    }

    public static TimeStampedKey genTimestamp(PrivateKey key) throws InvalidKeyException, SignatureException, NoSuchAlgorithmException
    {
        Instant timestamp = Instant.now();
        long timeStampLong = timestamp.getEpochSecond();
        String signedData = signData(String.valueOf(timeStampLong), key);
        String signedDataEncoded = base64urlencode(signedData);
        TimeStampedKey out = new TimeStampedKey(signedDataEncoded,timeStampLong);
        return out;
    }*/

    public static PublicKey decodeDSAPub(String key) throws NoSuchAlgorithmException, InvalidKeySpecException{
        byte[] byteKey = decodeBase64(key);
        X509EncodedKeySpec X509publicKey = new X509EncodedKeySpec(byteKey);
        KeyFactory kf = KeyFactory.getInstance("RSA");

        return kf.generatePublic(X509publicKey);
    }

    /**
     * Decodes a base64 encoded string to a byte array
     * @param value The base64 encoded string
     * @return The decoded byte array
     */
    public static byte[] decodeBase64(String value)
    {
        Decoder decoder = Base64.getDecoder();
        byte[] byteKey = decoder.decode(value.getBytes());
        return byteKey;
    }

    /**
     * Encodes a byte array as Base64 string
     * @param bytes The byte array to encode
     * @return The base64 encoded string
     */
    public static String encodeBase64(byte[] bytes)
    {
        Encoder encoder = Base64.getEncoder();
        byte[] out = encoder.encode(bytes);
        return new String(out);
    }

    /**
     * Encodes a string as Base64 string
     * @param string The string to encode
     * @return The base64 encoded string
     */
    public static String encodeBase64(String string)
    {
        return encodeBase64(string.getBytes());
    }

    public static boolean verify(String keyBase64, String signedData, String sigBase64) throws NoSuchAlgorithmException, InvalidKeySpecException, SignatureException, InvalidKeyException
    {
        //log.info("verifying " + keyBase64 + " : " + email + " : " + sigBase64);
        return verify(decodeDSAPub(keyBase64), signedData, sigBase64);
    }

    public static boolean verify(PublicKey key, String signedData, String sigBase64) throws SignatureException, NoSuchAlgorithmException, InvalidKeyException
    {
        Signature dsa = Signature.getInstance("SHA1withRSA");
        dsa.initVerify(key);
        dsa.update(signedData.getBytes());
        return dsa.verify(decodeBase64(sigBase64));
    }

}