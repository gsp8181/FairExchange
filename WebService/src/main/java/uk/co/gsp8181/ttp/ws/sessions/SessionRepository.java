package uk.co.gsp8181.ttp.ws.sessions;

import java.util.Date;

/**
 * Created by b1020537 on 20/03/2015.
 */
public interface SessionRepository {
    public Date addSession(Session session);

    public String getIp(String email);
}
