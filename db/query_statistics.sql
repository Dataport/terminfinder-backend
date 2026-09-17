SELECT customer.customername                       AS customer,
    TO_CHAR(appointment."yearmonth", 'Month YYYY') AS month,
    appointment.count                              AS appointments,
    participant.count                              AS participants,
    voting.count                                   AS votings
FROM "appointmentstatistic" appointment
         LEFT JOIN "participantstatistic" participant ON appointment."customerid" = participant."customerid"
         LEFT JOIN "votingstatistic" voting ON appointment."customerid" = voting."customerid"
         LEFT JOIN customer ON appointment."customerid" = customer.customerid
WHERE appointment."customerid" = '11111111-1111-1111-1111-111111111111'
ORDER BY appointment."yearmonth" DESC;