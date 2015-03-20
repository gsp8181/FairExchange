package uk.co.gsp8181.ttp.ws.sessions;

import org.mongodb.morphia.Datastore;
import org.mongodb.morphia.Morphia;
import org.mongodb.morphia.query.Query;
import org.mongodb.morphia.query.UpdateOperations;
import uk.co.gsp8181.ttp.db.MongoClientFactory;

import javax.inject.Inject;
import java.util.Calendar;
import java.util.Date;
import java.util.List;

/**
 * Created by b1020537 on 20/03/2015.
 */
public class SessionRepositoryMongo implements SessionRepository {

    //@Inject
    private static Datastore ds = MongoClientFactory.getInstance().getDatastore();


    @Override
    public List<Session> getActiveSessions() {
        Query q = ds.createQuery(Session.class).field("expires").greaterThan(new Date());
        List<Session> sessions = q.asList();
        return sessions;
        //List<Session> sessions = //ds.find(Session.class, "expires <", new Date());
    }

    @Override
    public Session addSession(Session s) {

        // Try and find the users previous sessions
        Query q = ds.createQuery(Session.class).field("email").equal(s.getEmail());
        Session old = (Session)q.get();
        if(old == null)
        {
            // Delete a previous session with the same IP
            Query q2 = ds.createQuery(Session.class).field("ip").equal(s.getIp());
            q2.field("email").notEqual(s.getEmail());
            ds.delete(q2);
            ds.save(s);
            return s;
        } else
        {
            old.setExpires(s.getExpires()); //TODO: is this the best way?
            UpdateOperations<Session> ops = ds.createUpdateOperations(Session.class).set("expires", s.getExpires());
            ds.update(q,ops);
            return old;
        }

    }

    @Override
    public String getIp(String email) {
        Query q = ds.createQuery(Session.class).field("email").equal(email);
        q.field("expires").greaterThan(new Date());

        Session s = (Session)q.get();

        if(s == null)
            return null;

        return s.getIp();

    }
}
