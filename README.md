1-- Table: public.customer_log

-- DROP TABLE IF EXISTS public.customer_log;

CREATE TABLE IF NOT EXISTS public.customer_log
(
    id integer NOT NULL DEFAULT nextval('customer_log_id_seq'::regclass),
    user_id integer NOT NULL,
    ip_address character varying(45) COLLATE pg_catalog."default",
    page_visited text COLLATE pg_catalog."default",
    action text COLLATE pg_catalog."default",
    "timestamp" timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    device_info text COLLATE pg_catalog."default",
    additional_info text COLLATE pg_catalog."default",
    CONSTRAINT customer_log_pkey PRIMARY KEY (id),
    CONSTRAINT fk_log_user FOREIGN KEY (user_id)
        REFERENCES public.user_m (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.customer_log
    OWNER to postgres;


2-- Table: public.usermillmapping

-- DROP TABLE IF EXISTS public.usermillmapping;

CREATE TABLE IF NOT EXISTS public.usermillmapping
(
    mappingid integer NOT NULL DEFAULT nextval('usermillmapping_mappingid_seq'::regclass),
    userid integer NOT NULL,
    millid integer NOT NULL,
    CONSTRAINT usermillmapping_pkey PRIMARY KEY (mappingid),
    CONSTRAINT usermillmapping_millid_fkey FOREIGN KEY (millid)
        REFERENCES public.mill (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT usermillmapping_userid_fkey FOREIGN KEY (userid)
        REFERENCES public.user_m (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.usermillmapping
    OWNER to postgres;

3- in user_m add region_code column
