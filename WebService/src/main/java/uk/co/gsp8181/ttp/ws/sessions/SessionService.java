package uk.co.gsp8181.ttp.ws.sessions;

import javax.inject.Inject;

/**
 * Created by b1020537 on 20/03/2015.
 */
public class SessionService {

    @Inject
    private SessionValidator validator;

    @Inject
    private SessionRepositoryMongo repository;
}
