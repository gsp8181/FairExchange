package uk.co.gsp8181.ttp.ws.sessions;

import org.mongodb.morphia.Datastore;
import org.mongodb.morphia.Morphia;
import uk.co.gsp8181.ttp.db.MongoClientFactory;

import javax.inject.Inject;
import java.util.Date;

/**
 * Created by b1020537 on 20/03/2015.
 */
public class SessionRepositoryMongo implements SessionRepository {

    @Inject
    private Datastore ds = MongoClientFactory.getInstance().getDatastore();

    @Override
    public Date addSession(Session session) {
        return null;
    }

    @Override
    public String getIp(String email) {
        return null;
    }
}
