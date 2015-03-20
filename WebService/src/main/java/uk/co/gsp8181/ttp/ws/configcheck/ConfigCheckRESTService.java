package uk.co.gsp8181.ttp.ws.configcheck;

import javax.ejb.Stateless;
import javax.servlet.http.HttpServletRequest;
import javax.ws.rs.Consumes;
import javax.ws.rs.GET;
import javax.ws.rs.Path;
import javax.ws.rs.Produces;
import javax.ws.rs.core.Context;
import javax.ws.rs.core.MediaType;
import java.io.IOException;
import java.net.Socket;

@Path("/config")
@Consumes(MediaType.APPLICATION_JSON)
@Produces(MediaType.APPLICATION_JSON)
@Stateless
public class ConfigCheckRESTService {

    @GET
    @Path("/ip")
    public String returnIp(@Context HttpServletRequest req)
    {
        String remoteAddr = req.getRemoteAddr();
        return remoteAddr;
    }

    @GET
    @Path("/port")
    public boolean isPortOpen(@Context HttpServletRequest req)
    {
        String remoteAddr = req.getRemoteAddr();
        try (Socket ignored = new Socket(remoteAddr, 6555)) {
            return true;
        } catch (IOException ignored) {
            return false;
        }
    }

}
