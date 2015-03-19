package uk.co.gsp8181.ttp.helloworld;

import javax.ws.rs.Consumes;
import javax.ws.rs.GET;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ejb.Stateless;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;

@Path("/hellorest")
@Consumes(MediaType.APPLICATION_JSON)
@Produces(MediaType.APPLICATION_JSON)
@Stateless
public class HelloWorldRESTService {


    /**
     * Hello, World!
     *
     * @return JSON wow
     */
    @GET
    @Path("/{param}")
    public Response getMsg(@PathParam("param") String msg) {
        HelloWorld response = new HelloWorld();
        response.setVal(msg);
        return Response.ok(response).build();
    }

}