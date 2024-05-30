import { TestBed } from '@angular/core/testing';

import { NecesidadService } from './necesidad.service';

describe('NecesidadService', () => {
  let service: NecesidadService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NecesidadService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
