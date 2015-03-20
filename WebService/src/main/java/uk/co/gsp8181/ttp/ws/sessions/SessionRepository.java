package uk.co.gsp8181.ttp.ws.sessions;

import java.util.Date;
import java.util.List;

/**
 * Created by b1020537 on 20/03/2015.
 */
public interface SessionRepository {
    public List<Session> getActiveSessions();

    public Session addSession(Session s);

    public String getIp(String email);
}
