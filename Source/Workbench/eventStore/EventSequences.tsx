// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { useBoolean } from '@fluentui/react-hooks';

import { EventHistogram } from './EventHistogram';
import { useState, useRef } from 'react';
import { FilterBuilder } from './FilterBuilder';
import { EventList } from './EventList';

import { EventSequenceInformation } from 'API/events/store/sequences/EventSequenceInformation';
import { AllEventSequences } from 'API/events/store/sequences/AllEventSequences';

import { AppendedEventWithJsonAsContent as AppendedEvent } from 'API/events/store/sequence/AppendedEventWithJsonAsContent';
import { AllEventTypes } from 'API/events/store/types/AllEventTypes';
import { GenerationSchemasForType } from 'API/events/store/types/GenerationSchemasForType';
import { EventTypeInformation } from 'API/events/store/types/EventTypeInformation';
import { TenantInfo } from 'API/configuration/tenants/TenantInfo';
import { AllTenants } from 'API/configuration/tenants/AllTenants';
import { useEffect } from 'react';
import { useRouteParams } from './RouteParams';

import { Button, FormControl, InputLabel, MenuItem, Select, Stack, Toolbar, Typography, Divider, Grid, Box, TextField } from '@mui/material';
import * as icons from '@mui/icons-material';
import { EventDetails } from './EventDetails';

export const EventSequences = () => {
    const { microserviceId } = useRouteParams();
    const [eventSequences] = AllEventSequences.use();
    const [tenants] = AllTenants.use();

    const [selectedEventSequence, setSelectedEventSequence] = useState<EventSequenceInformation>();
    const [selectedTenant, setSelectedTenant] = useState<TenantInfo>();

    const [isTimelineOpen, { toggle: toggleTimeline }] = useBoolean(false);
    const [isFilterOpen, { toggle: toggleFilter }] = useBoolean(false);

    const [selectedEvent, setSelectedEvent] = useState<AppendedEvent | undefined>(undefined);
    const [selectedEventType, setSelectedEventType] = useState<EventTypeInformation | undefined>(undefined);

    const [eventTypes, refreshEventTypes] = AllEventTypes.use({
        microserviceId: microserviceId
    });

    const refreshEventsCallback = useRef<() => void>();
    function registerRefreshEventsCallback(callback: () => void) {
        refreshEventsCallback.current = callback;
    }

    const getGenerationalSchemasForTypeArguments = () => {
        return {
            microserviceId,
            eventTypeId: selectedEventType!.identifier
        };
    };

    const [schema, reloadGenerationalSchemas] = GenerationSchemasForType.use();

    useEffect(() => {
        if (eventSequences.data.length > 0) {
            setSelectedEventSequence(eventSequences.data[0]);
        }
    }, [eventSequences.data]);

    useEffect(() => {
        if (tenants.data.length > 0) {
            setSelectedTenant(tenants.data[0]);
        }
    }, [tenants.data]);

    const eventSelected = (item: AppendedEvent) => {
        if (item !== selectedEvent) {
            const eventType = eventTypes.data.find(_ => _.identifier == item.metadata.type.id);
            setSelectedEvent(item);
            setSelectedEventType(eventType);
            reloadGenerationalSchemas({
                microserviceId,
                eventTypeId: eventType!.identifier
            });
        }
    };

    return (
        <>
            <Stack direction='column' style={{ height: '100%' }}>
                <Typography variant='h4'>Event sequences</Typography>
                <Divider sx={{ mt: 1, mb: 3 }} />
                <Toolbar>
                    <FormControl size='small' sx={{ m: 1, minWidth: 120 }}>
                        <InputLabel>Sequence</InputLabel>
                        <Select
                            label='Sequence'
                            autoWidth
                            value={selectedEventSequence?.id || ''}
                            onChange={e => setSelectedEventSequence(eventSequences.data.find(_ => _.id == e.target.value))}>

                            {eventSequences.data.map(sequence => {
                                return (
                                    <MenuItem key={sequence.id} value={sequence.id}>{sequence.name}</MenuItem>
                                );
                            })}
                        </Select>
                    </FormControl>

                    <FormControl size='small' sx={{ m: 1, minWidth: 120 }}>
                        <InputLabel>Tenant</InputLabel>
                        <Select
                            label='Tenant'
                            autoWidth
                            value={selectedTenant?.id || ''}
                            onChange={e => setSelectedTenant(tenants.data.find(_ => _.id == e.target.value))}>

                            {tenants.data.map(tenant => {
                                return (
                                    <MenuItem key={tenant.id} value={tenant.id}>{tenant.name}</MenuItem>
                                );
                            })}
                        </Select>
                    </FormControl>

                    {/* <Button startIcon={<icons.Timeline />} onClick={() => {
                        toggleTimeline();
                        if (isFilterOpen) toggleFilter();
                    }}>Timeline</Button>
                    <Button startIcon={<icons.FilterAlt />} onClick={() => {
                        toggleFilter();
                        if (isTimelineOpen) toggleTimeline();
                    }}>Filter</Button> */}
                    <Button startIcon={<icons.PlayArrow />}
                        onClick={() => refreshEventsCallback.current?.()}>Run</Button>

                    {isTimelineOpen &&
                        <Button startIcon={<icons.ZoomOutMap />} onClick={() => {
                        }}>Reset Zoom</Button>}
                </Toolbar>
                <div>
                    {isTimelineOpen && <EventHistogram eventLog={selectedEventSequence!.id} />}
                    {isFilterOpen && <FilterBuilder />}
                </div>
                {(selectedEventSequence && selectedTenant) &&
                    <Box sx={{ height: '100%', flex: 1 }}>
                        <Grid container spacing={2} sx={{ height: '100%' }}>
                            <Grid item xs={8}>
                                <EventList
                                    registerRefreshEvents={registerRefreshEventsCallback}
                                    eventSequenceId={selectedEventSequence.id}
                                    tenantId={selectedTenant!.id}
                                    microserviceId={microserviceId}
                                    eventTypes={eventTypes.data} onEventSelected={eventSelected}
                                    onEventsRedacted={() => { }}
                                    sequenceNumber={selectedEventSequence!.id} />
                            </Grid>

                            <Grid item xs={4} >
                                {selectedEvent &&
                                    <Box>
                                        <Typography variant='h6'>{selectedEventType?.name}</Typography>
                                        <FormControl size='small' sx={{ m: 1, minWidth: '90%' }}>
                                            <TextField label='Occurred' disabled
                                                defaultValue={(selectedEvent?.context.occurred || new Date()).toISOString().toLocaleString()} />
                                        </FormControl>
                                        <EventDetails event={selectedEvent} type={selectedEventType!} schema={schema.data} />

                                        {/* {
                                            (selectedEvent && selectedEvent.content) && Object.keys(selectedEvent.content).map(_ =>
                                                <FormControl key={_} size='small' sx={{ m: 1, minWidth: '90%' }}>
                                                    <TextField
                                                        label={_} disabled defaultValue={selectedEvent!.content[_]} />
                                                </FormControl>)
                                        }
 */}
                                    </Box>
                                }
                            </Grid>
                        </Grid>
                    </Box>}
            </Stack>
        </>
    );
};
