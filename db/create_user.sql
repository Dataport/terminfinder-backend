CREATE USER terminfinderadmin WITH
	LOGIN
	NOSUPERUSER
	INHERIT
	NOREPLICATION;
ALTER USER "terminfinderadmin" WITH PASSWORD 'terminfinderadmin';

ALTER USER terminfinderadmin CREATEDB;
