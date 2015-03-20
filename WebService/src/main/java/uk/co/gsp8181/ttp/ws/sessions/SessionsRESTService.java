package uk.co.gsp8181.ttp.ws.sessions;

import uk.co.gsp8181.ttp.ws.helloworld.HelloWorld;

import javax.ejb.Stateless;
import javax.inject.Inject;
import javax.ws.rs.*;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;

/**
 * Created by b1020537 on 19/03/2015.
 */
@Path("/sessions")
@Consumes(MediaType.APPLICATION_JSON)
@Produces(MediaType.APPLICATION_JSON)
@Stateless
public class SessionsRESTService {

    @Inject
    private SessionService service;

    @GET
    @Path("/{email}")
    public Response getIp(@PathParam("email") String msg) {
        HelloWorld response = new HelloWorld();
        response.setVal(msg);
        return Response.ok(response).build();
    }

    @POST
    public Response startSession(String email)
    {
        HelloWorld response = new HelloWorld();
        response.setVal(email);
        return Response.ok(response).build();
    }
}
