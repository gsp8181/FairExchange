package uk.co.gsp8181.ttp.ws.configcheck;

import uk.co.gsp8181.ttp.sec.CertificateTools;
import uk.co.gsp8181.ttp.sec.TestData;

import javax.ejb.Stateless;
import javax.servlet.http.HttpServletRequest;
import javax.ws.rs.*;
import javax.ws.rs.core.Context;
import javax.ws.rs.core.MediaType;
import java.io.IOException;
import java.net.Socket;
import java.security.*;

@Path("/config")
//@Consumes(MediaType.APPLICATION_JSON)
@Produces(MediaType.APPLICATION_JSON)
@Stateless
public class ConfigCheckRESTService {

    @GET
    @Path("/ip")
    public /*String*/ Ip returnIp(@Context HttpServletRequest req)
    {
        return new Ip(req.getRemoteAddr());
        //return req.getRemoteAddr();
    }

    @GET
    @Path("/cert")
    public TestData returnTestData()
    {
        try {
            TestData td =  CertificateTools.getTestData("wazzup");
            PublicKey pub = CertificateTools.decodeDSAPub(td.getPublicKeyBase64());
            String pem = CertificateTools.getPemForKey(pub);
            return new TestData(pem,td.getEmail(),td.getSigBase64(),td.getPrivateKeyBase64());
        } catch (Exception e)
        {
            throw new WebApplicationException(e);
        }
    }

    @GET
    @Path("/port")
    public /*boolean*/ PortOpen isPortOpen(@Context HttpServletRequest req)
    {
        String remoteAddr = req.getRemoteAddr();
        try (Socket ignored = new Socket(remoteAddr, 6555)) {
            return new PortOpen(remoteAddr, 6555, true);
            //return true;
        } catch (IOException ignored) {
            return new PortOpen(remoteAddr, 6555, false);
            //return false;
        }
    }

    @GET
    @Path("/port/{port}")
    public /*boolean*/ PortOpen isPortOpen(@Context HttpServletRequest req, @PathParam("port") int port)
    {
        String remoteAddr = req.getRemoteAddr();
        try (Socket ignored = new Socket(remoteAddr, port)) {
            return new PortOpen(remoteAddr, port, true);
            //return true;
        } catch (IOException ignored) {
            return new PortOpen(remoteAddr, port, false);
            //return false;
        }
    }

}
