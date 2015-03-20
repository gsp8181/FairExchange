package uk.co.gsp8181.ttp.ws.sessions;

import org.apache.commons.lang.time.DateUtils;

import javax.inject.Inject;
import java.time.Instant;
import java.time.LocalDateTime;
import java.util.Date;
import java.util.List;

/**
 * Created by b1020537 on 20/03/2015.
 */
public class SessionService {

    //@Inject
    private static SessionValidator validator = new SessionValidator();

    //@Inject
    private static SessionRepository repository = new SessionRepositoryMongo();


    public boolean start(String email, String remoteAddr) {
        Session s = new Session();
        //TODO: is there an active session out?
        s.setEmail(email);
        s.setIp(remoteAddr);
        Date expires = DateUtils.addMinutes(new Date(), 30);
        s.setExpires(expires);
        Session added = repository.addSession(s);
        return (added.getId() != null);
    }

    public List<Session> getActiveSessions()
    {
        return repository.getActiveSessions();
    }

    public String getSessionIp(String email) {
        return repository.getIp(email);
    }
}
