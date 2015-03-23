package uk.co.gsp8181.ttp.ws.sessions;

import javax.ejb.Stateless;
import javax.inject.Inject;
import javax.servlet.http.HttpServletRequest;
import javax.ws.rs.*;
import javax.ws.rs.core.Context;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * Created by b1020537 on 19/03/2015.
 */
@Path("/sessions")
//@Consumes(MediaType.APPLICATION_JSON) //TODO: and text plain? or does this need moving down
@Produces(MediaType.APPLICATION_JSON)
@Stateless
public class  SessionsRESTService {

    //@Inject
    private static SessionService service = new SessionService();

    @GET
    @Path("/{email}")
    public Response getIp(@PathParam("email") String email) {
        String ip = service.getSessionIp(email);
        if(ip != null)
            return Response.ok(ip).build();
        throw new WebApplicationException(Response.Status.NOT_FOUND); //TODO: error message
    }

    @GET
    @Path("/")
    public Response getActiveSessions()
    {
        List<Session> sessions = service.getActiveSessions();
        return Response.ok(sessions).build();
    }

    @POST
    public Response startSession(String email, @Context HttpServletRequest req)
    {
        String remoteAddr = req.getRemoteAddr();
        boolean started = service.start(email, remoteAddr);
        Map<String, Boolean> response = new HashMap<>(); //TODO: needs fix
        response.put("started",started);
        return Response.ok(response).build();
    }
}
